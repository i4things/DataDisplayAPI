//
//  DataDisplay.swift
//  I4Things
//
//  Created by Emil on 4/19/19.
//  Copyright © 2019 B2N. All rights reserved.
//

import Foundation

public enum DataDisplayError: Error {
  case error(description: String)
}

public class DataDisplay {
  public typealias DataDisplayResult = Result<String, Error>
  public typealias DataCompletion = (DataDisplayResult) -> Void
  
  let server: URL
  
  public init(server: URL) {
    self.server = server
  }
  
  public convenience init() {
    self.init(server: URL(string: "http://server.i4things.com:5408/")!)
  }
  
  private typealias Byte = UInt8

  private static let resultStart = 16
  private static let resultEnd = 2
  

  private static func toByteArray<T: FixedWidthInteger & UnsignedInteger>(_ u: T) -> [Byte] {
    let numBytes = MemoryLayout<T>.size
    var r = Array<Byte>(repeating: 0, count: numBytes)
    var l = u
    for i in 0 ..< r.count {
      r[i] = UInt8(truncatingIfNeeded: l)
      l >>= 8
    }
    return r
  }
  
  private static func CRC4(_ c: [Byte]) -> UInt32 {
    var crc: UInt32 = 0
    for i in 0 ..< c.count {
      crc = (crc << 1) ^ UInt32(c[i])
    }
    return crc
  }

  private static func challenge(networkKey: Data) -> Data {
    let now = Date()
    let nowTI = UInt64(now.timeIntervalSince1970 * 1000)
    let nowBytes = toByteArray(nowTI)
    let crc = CRC4(nowBytes)
    let crcBytes = toByteArray(crc)
    assert(crcBytes.count == 4)
    let buf = crcBytes + nowBytes
    let data = Data(buf)
    let encr = XXTEA.encrypt(data, key: networkKey)
    return encr
  }

  // private typealias PathMaker = () -> String
  
  // id integer. network_key in HEX format 32 chars
  private static func dataRequestPath(id: UInt64, networkKey: Data) -> String {
    let ch = challenge(networkKey: networkKey)
    let path = String(id) + "-" + I4TUtils.hexStringFromData(ch).uppercased()
    return path
  }

  private static func dataHistRequestPath(id: UInt64, networkKey: Data, day: Byte) -> String {
    let ch = challenge(networkKey: networkKey)
    let path = String(id) + "-" + String(day) + "-" + I4TUtils.hexStringFromData(ch).uppercased()
    return path
  }

  private static func dataFromRequestPath(id: UInt64, networkKey: Data, timestamp: UInt64) -> String {
    let ch = challenge(networkKey: networkKey)
    let path = String(id) + "-" + String(timestamp) + "-" + I4TUtils.hexStringFromData(ch).uppercased()
    return path
  }

  private static func nodeId(siteId: UInt32, deviceId: UInt32) -> UInt64 {
    return (UInt64(siteId) << 16) | UInt64(deviceId)
  }
  
  private func logDebug(_ str: @autoclosure () -> String) {
    print("I4THINGS [Debug]: " + str())
  }
  
  private func processResponse(data: Data?,
                               response: URLResponse?,
                               error: Error?,
                               completion: DataCompletion) {
    if let error = error {
      completion(.failure(error))
      return
    }
    
    guard let httpURLResponse = response as? HTTPURLResponse else {
      completion(.failure(DataDisplayError.error(description: "Invalid response")))
      return
    }
    
    guard httpURLResponse.statusCode == 200 else {
      completion(.failure(DataDisplayError.error(description: "HTTP Status Code: \(httpURLResponse.statusCode)")))
      return
    }
    
    guard let data = data else {
      completion(.failure(DataDisplayError.error(description: "Missing data")))
      return
    }
    
    guard let s = String(data: data, encoding: .utf8) else {
      completion(.failure(DataDisplayError.error(description: "Bad encoding")))
      return
    }
    
    let str = s.trimmingCharacters(in: .whitespacesAndNewlines)
    
    if str.count < DataDisplay.resultStart + DataDisplay.resultEnd {
      completion(.failure(DataDisplayError.error(description: "Bad data")))
      return
    }

    let startIndex = str.index(str.startIndex, offsetBy: DataDisplay.resultStart)
    let endIndex = str.index(str.endIndex, offsetBy: -DataDisplay.resultEnd)
    let json = str[startIndex ..< endIndex]
    completion(.success(String(json)))
  }
  
  /// Use to request/receive all data for the current day from the server
  ///
  /// - Parameters:
  ///   - siteId:
  ///   - deviceId:
  ///   - networkKey: Network key (HEX format 32 chars)
  ///   - completion: Invoked on arbitrary (background) thread
  public func getData(siteId: UInt32,
                      deviceId: UInt32,
                      networkKey: String,
                      completion: @escaping DataCompletion) {
    guard networkKey.utf8.count == 32 else {
      fatalError("Invalid network key (must be 32 characters)")
    }
    
    guard let key = I4TUtils.dataFromHexString(networkKey) else {
      fatalError("Invalid network key (non hex digits)")
    }
    
    let nodeId = DataDisplay.nodeId(siteId: siteId, deviceId: deviceId)
    let path = DataDisplay.dataRequestPath(id: nodeId, networkKey: key)
    
    let url = server.appendingPathComponent("iot_get/" + path)
  
    logDebug("URL: \(url.absoluteString)")
    
    URLSession.shared.dataTask(with: url) {
      data, response, error in
      self.processResponse(data: data, response: response, error: error, completion: completion)
    }.resume()
  }

  /// Use to request/receive only last data received from the node
  ///
  /// - Parameters:
  ///   - siteId:
  ///   - deviceId:
  ///   - networkKey: Network key (HEX format 32 chars)
  ///   - completion: Invoked on arbitrary (background) thread
  public func getLast(siteId: UInt32,
                      deviceId: UInt32,
                      networkKey: String, completion: @escaping DataCompletion) {
    guard networkKey.utf8.count == 32 else {
      fatalError("Invalid network key (must be 32 characters)")
    }
    
    guard let key = I4TUtils.dataFromHexString(networkKey) else {
      fatalError("Invalid network key (non hex digits)")
    }
    
    let nodeId = DataDisplay.nodeId(siteId: siteId, deviceId: deviceId)
    let path = DataDisplay.dataRequestPath(id: nodeId, networkKey: key)
    
    let url = server.appendingPathComponent("iot_get_last/" + path)
    
    logDebug("URL: \(url.absoluteString)")
    
    URLSession.shared.dataTask(with: url) {
      data, response, error in
      self.processResponse(data: data, response: response, error: error, completion: completion)
      }.resume()
  }

  /// Use to request/receive history data for specific day
  ///
  /// - Parameters:
  ///   - siteId:
  ///   - deviceId:
  ///   - day: Day for which the data is requested - 0 yesterday, 1 - the day before yesterday etc.
  ///   - networkKey: Network key (HEX format 32 chars)
  ///   - completion: Invoked on arbitrary (background) thread
  public func getHist(siteId: UInt32, deviceId: UInt32, day: Int, networkKey: String, completion: @escaping DataCompletion) {
    guard networkKey.utf8.count == 32 else {
      fatalError("Invalid network key (must be 32 characters)")
    }
    
    guard let key = I4TUtils.dataFromHexString(networkKey) else {
      fatalError("Invalid network key (non hex digits)")
    }
    
    let nodeId = DataDisplay.nodeId(siteId: siteId, deviceId: deviceId)
    let path = DataDisplay.dataHistRequestPath(id: nodeId, networkKey: key, day: Byte(truncatingIfNeeded: day))
    
    let url = server.appendingPathComponent("iot_get_hist/" + path)
    
    logDebug("URL: \(url.absoluteString)")
    
    URLSession.shared.dataTask(with: url) {
      data, response, error in
      self.processResponse(data: data, response: response, error: error, completion: completion)
      }.resume()
  }
}

