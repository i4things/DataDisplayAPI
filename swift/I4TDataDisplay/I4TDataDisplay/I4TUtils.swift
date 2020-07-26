//
//  I4TUtils.swift
//  I4Things
//
//  Created by Emil on 6/3/19.
//  Copyright © 2019 B2N. All rights reserved.
//

import Foundation

public class I4TUtils {
  
  // https://github.com/krzyzanowskim/CryptoSwift
  
  public static func dataFromHexString(_ hexString: String) -> Data? {
    let length = hexString.count / 2
    var data = Data(capacity: length)
    for i in 0 ..< length {
      let j = hexString.index(hexString.startIndex, offsetBy: i * 2)
      let k = hexString.index(j, offsetBy: 2)
      let bytes = hexString[j..<k]
      if let byte = UInt8(bytes, radix: 16) {
        data.append(byte)
      } else {
        return nil
      }
    }
    return data
  }
  
  public static func hexStringFromData(_ data: Data) -> String {
    return data.reduce("") {
      var s = String($1, radix: 16)
      if s.count == 1 {
        s = "0" + s
      }
      return $0 + s
    }
  }
}
