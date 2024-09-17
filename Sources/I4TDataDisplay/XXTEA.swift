//
//  XXTEA.swift
//  I4Things
//
//  Created by Emil on 5/6/19.
//  Copyright © 2019 B2N. All rights reserved.
//

import Foundation

 /**********************************************************\
 |                                                          |                                             |
 | Based on                                                 |
 | XXTEA encryption algorithm library for .NET.             |
 |                                                          |
 | Encryption Algorithm Authors:                            |
 |      David J. Wheeler                                    |
 |      Roger M. Needham                                    |
 |                                                          |
 | Code Author:  Ma Bingyao <mabingyao@gmail.com>           |
 | LastModified: Mar 10, 2015                               |
 | Part modified by i4things                                |
 |                                                          |
 \**********************************************************/

public class XXTEA {

  public static func encrypt(_ data: Data, key: Data) -> Data {
    if data.count == 0 {
        return data
    }
    var arrData = toUInt32ArraySize(data)
    let arrKey = toUInt32Array(key)
    return toData(encryptArray(&arrData, key: arrKey))
  }

  public static func decrypt(_ data: Data, key: Data) -> Data {
    if data.count == 0 {
        return data
    }
    assert(key.count == 16)
    
    var arrData = toUInt32Array(data)
    let arrKey = toUInt32Array(key)
    return toDataSize(decryptArray(&arrData, key: arrKey));
  }

  // MARK: - Private
  
  private static func toUInt32ArraySize(_ data: Data) -> [UInt32] {
    var newData = data
    newData.insert(UInt8(data.count), at: 0)
    while newData.count < 8 || newData.count & 3 != 0 {
      newData.append(UInt8.random(in: 0...255))
    }

    return toUInt32Array(newData)
  }

  private static func toUInt32Array(_ data: Data) -> [UInt32] {
    let length = data.count
    let n = (((length & 3) == 0) ? (length >> 2) : ((length >> 2) + 1))
    var result = Array<UInt32>(repeating: 0, count: n)
    for i in 0 ..< length {
      result[i >> 2] |= UInt32(data[i]) << ((i & 3) << 3);
    }
    return result
  }

  private static func toData(_ arr: [UInt32]) -> Data {
    let n = arr.count * 4
    var d = Data(count: n)
    for i in 0 ..< n {
      d[i] = UInt8(truncatingIfNeeded: arr[i >> 2] >> ((i & 3) << 3))
    }
    return d
  }

  private static func toDataSize(_ arr: [UInt32]) -> Data {
    let data = toData(arr)
    if data.count < 1 {
      return data
    }
    let size = Int(data[0])
    let r = data.subdata(in: 1 ..< size + 1)
    return r
  }
  
  private static let delta: UInt32 = 0x9E3779B9

  private static func MX(_ sum: UInt32, _ y: UInt32, _ z: UInt32, _ p: Int32, _ e: UInt32, _ k : [UInt32]) -> UInt32 {
    
    // return (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);

    let v1 = (z >> 5 ^ y << 2)
    let v2 = (y >> 3 ^ z << 4)
    let ix = Int(UInt32(p) & 3 ^ e)
    let v3 = (sum ^ y) &+ (k[ix] ^ z)
    return (v1 &+ v2) ^ v3
  }

  private static func encryptArray(_ v: inout [UInt32], key: [UInt32]) -> [UInt32] {
    let n = v.count - 1
    if n < 1 {
        return v
    }
    var z: UInt32 = v[n]
    var y: UInt32
    var sum: UInt32 = 0
    var e: UInt32
    
    let q = Int32(6 + 52 / (n + 1))
    
    for _ in 0 ..< q {
      sum = sum &+ delta
      e = sum >> 2 & 3
      for p in 0 ..< n {
        y = v[p + 1]
        let mx = MX(sum, y, z, Int32(p), e, key)
        v[p] = v[p] &+ mx
        z = v[p]
      }
      y = v[0]
      v[n] = v[n] &+ MX(sum, y, z, Int32(n), e, key)
      z = v[n]
    }
    return v
  }

  private static func decryptArray(_ v: inout [UInt32], key: [UInt32]) -> [UInt32] {
    let n = v.count - 1
    if n < 1 {
        return v
    }
    
    var z: UInt32
    var y = v[0]
    var e: UInt32
    let q = UInt32(6 + 52 / (n + 1))
    var sum = UInt32(q &* delta)
    
    while sum != 0 {
      e = sum >> 2 & 3
      for p in (1...n).reversed() {
        z = v[p - 1]
        v[p] = v[p] &- MX(sum, y, z, Int32(p), e, key)
        y = v[p]
      }
      z = v[n]
      v[0] = v[0] &- MX(sum, y, z, 0, e, key)
      y = v[0]
      sum = sum &- delta
    }
    return v;
  }
}
