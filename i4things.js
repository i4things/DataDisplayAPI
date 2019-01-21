
/**********************************************************\
|                                                          |
| xxtea.js                                                 |
|                                                          |
| XXTEA encryption algorithm library for JavaScript.       |
|                                                          |
| Encryption Algorithm Authors:                            |
|      David J. Wheeler                                    |
|      Roger M. Needham                                    |
|                                                          |
| Code Author: Ma Bingyao <mabingyao@gmail.com>            |
| LastModified: Oct 4, 2016                                |
| (partially edited and cropped)                           |
|                                                          |
| License (MIT) 										   |
\**********************************************************/
var i4things_xxtea_delta = 0x9E3779B9;

function i4things_xxtea_mx(sum, y, z, p, e, k) {
    return ((z >>> 5 ^ y << 2) + (y >>> 3 ^ z << 4)) ^ ((sum ^ y) + (k[p & 3 ^ e] ^ z));
}

function i4things_xxtea_int32(i) {
    return i & 0xFFFFFFFF;
}


function i4things_xxtea_toUint32Array(bs) {
    var length = bs.length;
    var n = length >> 2;
    v = new Array(n);
    for (var i = 0; i < n; ++i) {
        var j = i << 2;
        v[i] = ((bs[j + 3] & 0xFF) << 24) | ((bs[j + 2] & 0xFF) << 16) | ((bs[j + 1] & 0xFF) << 8) | (bs[j] & 0xFF);
    }
    return v;
}


function i4things_xxtea_toUint32ArraySize(bs) {
    bs.unshift(bs.length);
    for (var i = 0;
        ((bs.length < 8) || ((bs.length & 3) != 0)); i++) {
        bs.push(Math.floor((Math.random() * 255)));
    }

    return i4things_xxtea_toUint32Array(bs);
}


function i4things_xxtea_toUint8ArraySize(v) {
    var bs = new Array(v.length << 2);
    for (var i = 0; i < v.length; i++) {
        j = (i << 2);
        bs[j + 3] = v[i] >> 24 & 0xFF;
        bs[j + 2] = v[i] >> 16 & 0xFF;
        bs[j + 1] = v[i] >> 8 & 0xFF;
        bs[j] = v[i] & 0xFF;
    }

    return bs.slice(1, bs[0] + 1);
}

function i4things_xxtea_toUint8Array(v) {
    var bs = new Array(v.length << 2);
    for (var i = 0; i < v.length; i++) {
        j = (i << 2);
        bs[j + 3] = v[i] >> 24 & 0xFF;
        bs[j + 2] = v[i] >> 16 & 0xFF;
        bs[j + 1] = v[i] >> 8 & 0xFF;
        bs[j] = v[i] & 0xFF;
    }

    return bs;
}

function i4things_xxtea_encryptUint32Array(v, k) {
    var length = v.length;
    var n = length - 1;
    var y, z, sum, e, p, q;
    z = v[n];
    sum = 0;
    for (q = Math.floor(6 + 52 / length) | 0; q > 0; --q) {
        sum = i4things_xxtea_int32(sum + i4things_xxtea_delta);
        e = sum >>> 2 & 3;
        for (p = 0; p < n; ++p) {
            y = v[p + 1];
            z = v[p] = i4things_xxtea_int32(v[p] + i4things_xxtea_mx(sum, y, z, p, e, k));
        }
        y = v[0];
        z = v[n] = i4things_xxtea_int32(v[n] + i4things_xxtea_mx(sum, y, z, n, e, k));
    }
    return v;
}

function i4things_xxtea_decryptUint32Array(v, k) {
    var length = v.length;
    var n = length - 1;
    var y, z, sum, e, p, q;
    y = v[0];
    q = Math.floor(6 + 52 / length);
    for (sum = i4things_xxtea_int32(q * i4things_xxtea_delta); sum !== 0; sum = i4things_xxtea_int32(sum - i4things_xxtea_delta)) {
        e = sum >>> 2 & 3;
        for (p = n; p > 0; --p) {
            z = v[p - 1];
            y = v[p] = i4things_xxtea_int32(v[p] - i4things_xxtea_mx(sum, y, z, p, e, k));
        }
        z = v[n];
        y = v[0] = i4things_xxtea_int32(v[0] - i4things_xxtea_mx(sum, y, z, 0, e, k));
    }
    return v;
}

function i4things_xxtea_decrypt(data, key) {
    if (data === undefined || data === null || data.length === 0) {
        return data;
    }
    return i4things_xxtea_toUint8ArraySize(i4things_xxtea_decryptUint32Array(i4things_xxtea_toUint32Array(data), i4things_xxtea_toUint32Array(key)));
}

function i4things_xxtea_encrypt(data, key) {
    if (data === undefined || data === null || data.length === 0) {
        return data;
    }
    return i4things_xxtea_toUint8Array(i4things_xxtea_encryptUint32Array(i4things_xxtea_toUint32ArraySize(data), i4things_xxtea_toUint32Array(key)));
}


/**********************************************************\
				  Data Operations
\**********************************************************/
function i4things_parse_hex(str) {
    var result = [];
    while (str.length >= 2) {
        result.push(parseInt(str.substring(0, 2), 16));
        str = str.substring(2, str.length);
    }

    return result;
}

function i5things_to_hex(arr) {
    return Array.from(arr, function(byte) {
        return ('0' + (byte & 0xFF).toString(16)).slice(-2);
    }).join('')
}

function i4things_long_to_byte_array( /*long*/ l) {
    var ba = [0, 0, 0, 0, 0, 0, 0, 0];

    for (var i = 0; i < ba.length; i++) {
        var b = l & 0xFF;
        ba[i] = b;
        l = (l - b) / 256;
    }

    return ba;
};

function i4things_byte_array_to_long( /*byte[]*/ ba) {
    var v = 0;
    for (var i = ba.length - 1; i >= 0; i--) {
        v = (v * 256) + ba[i];
    }

    return v;
};

function i4things_long_random(length) {
    var n = '';
    while (n.length < length) {
        n += ('' + Math.random()).split('.')[1];
    }
    return n.substr(0, length);
}

function i4things_crc4(c) {
    var crc = 0;
    for (var i = 0; i < c.length; i++) {
        var b = c[i] & 0xFF;
        crc = (crc << 1) ^ b;
        crc = crc & 0xFFFFFFFF;
    };
    return crc;
}

function i4things_crc(c) {
    var crc = 8606;
    for (var i = 0; i < c.length; i++) {
        var b = c[i] & 0xFF;
        crc = (crc << 1) ^ b;
        crc = crc & 0xFFFFFFFF;
    };
    return crc & 0xFF;
}

function i4_things_challenge(network_key) {
    var t = new Date().getTime();
    var c = i4things_long_to_byte_array(t);
    // gen CRC
    var crc = i4things_crc4(c);

    // and in front of challenge
    c.unshift(crc & 0xFF, crc >> 8 & 0xFF, crc >> 16 & 0xFF, crc >> 24 & 0xFF);

    // encrypt
    return i4things_xxtea_encrypt(c, i4things_parse_hex(network_key));
}

// id integer. network_key in HEX format 32 chars
function i4things_get_data_request(id, network_key) {
    // gen challenge
    var c = i4_things_challenge(network_key);

    return id.toString() + '-' + i5things_to_hex(c).toUpperCase() + '-' + i4things_long_random(20);
}

// id integer, data byte array, network_key in HEX format 32 chars, private_key 16 bytes array
function i4things_set_data_request(id, data, network_key, private_key) {
    // gen challenge
    var c = i4_things_challenge(network_key);
    var crc = i4things_crc(data);
    data.unshift(crc);

    return id.toString() + '-' + i5things_to_hex(c).toUpperCase() + '-' + i5things_to_hex(i4things_xxtea_encrypt(data, private_key)).toUpperCase() + "-" + i4things_long_random(20);

}

// id integer, hist day index integer, network_key in HEX format 32 chars
function i4things_get_data_hist_request(id, day_idx, network_key) {
    // gen challenge
    var c = i4_things_challenge(network_key);

    return id.toString() + '-' + day_idx.toString() + '-' + i5things_to_hex(c).toUpperCase() + '-' + i4things_long_random(20);
}

/**********************************************************\
				  Dynamic load script functions
\**********************************************************/
function i4things_load_script(url, callback) {

    var script = document.createElement("script")
    script.type = "text/javascript";

    if (script.readyState) { //IE
        script.onreadystatechange = function() {
            if (script.readyState == "loaded" ||
                script.readyState == "complete") {
                script.onreadystatechange = null;
                callback();
            }
        };
    } else { //Others
        script.onload = function() {
            callback();
        };
    }

    script.src = url;
    document.getElementsByTagName("head")[0].appendChild(script);
}


// return
//{
//	pos: new_pos,
//	value:new_value 
//};
function i4things_get_ots(buf, pos) {
    var start = pos;
    switch (buf[start] & 0x3) {
        case 0:
            {
                var new_pos = pos + 1;
                var new_value = (buf[start] >> 2) & 0x3F;
                return {
                    pos: new_pos,
                    value: new_value
                };
            }
        case 1:
            {
                var new_pos = pos + 2;
                var new_value = ((((buf[start + 1] << 8) | (buf[start])) >> 2) & 0x3FFF);
                return {
                    pos: new_pos,
                    value: new_value
                };
            }
        case 2:
            {
                var new_pos = pos + 4;
                var new_value = ((buf[start + 3] << 24) | (buf[start + 2] << 16) | (buf[start + 1] << 8) | (buf[start] >> 2) & 0x3FFFFFFF);
                return {
                    pos: new_pos,
                    value: new_value
                };
            }
        case 3:
            {
                var new_pos = pos + 8;
                var new_value = (((buf[start + 7] << 24) | (buf[start + 6] << 16) | (buf_[start + 5] << 8) | (buf[start + 4] >> 2) & 0x3FFFFFFF) * 0xFFFFFFFF) +
                    ((buf[start + 3] << 24) | (buf[start + 2] << 16) | (buf_[start + 1] << 8) | (buf[start] >> 2));
                return {
                    pos: new_pos,
                    value: new_value
                };
            }
    }
}

function i4things_put_ots(v, buf) {
    v = (v * 4);
    if (v < 0x40) {
        buf.push(v & 0xFF);
    } else if (v < 0x4000) {
        v = v | 1;
        buf.push(v & 0xFF, (v >> 8) & 0xFF);
    } else if (v < 0x40000000) {
        v = v | 2;
        buf.push(v & 0xFF, (v >> 8) & 0xFF, (v >> 16) & 0xFF, (v >> 24) & 0xFF);
    } else if (v < 0x4000000000000000) {
        v = v + 3;
        buf.push(v & 0xFF, (v >> 8) & 0xFF, (v >> 16) & 0xFF, (v >> 24) & 0xFF);
        v = v / 0xFFFFFFFF;
        buf.push(v & 0xFF, (v >> 8) & 0xFF, (v >> 16) & 0xFF, (v >> 24) & 0xFF);
    }
}
/**********************************************************\
				  Value helper functions
\**********************************************************/

//add discrete data
//use this if you want for example to store decimal(floating-point) temperature between -20 and +60 in one byte
// pos is the position from which the data will be written
// e.g. : add_discrete(buf, 0,  -20.0, 60.0, 23.65, 1);
//you need to ensure that you at least have container_size bytes available in the buffer
//1 <= conainer_size <= 4
//on return pos will have the value of first free byte(next postion after the data)

function i4things_add_discrete(buf, min, max, value, container_size) {
    var dev;
    switch (container_size) {
        case 1:
            {
                dev = (max - min) / 255.0;
                var dis = Math.round((value - min) / dev);
                buf.push(dis & 0xFF);
            };
            break;
        case 2:
            {
                dev = (max - min) / 65535.0;
                var dis = Math.round((value - min) / dev);
                buf.push(dis & 0xFF, dis >> 8 & 0xFF);
            };
            break;
        case 3:
            {
                dev = (max - min) / 16777215.0;
                var dis = Math.round((value - min) / dev);
                buf.push(dis & 0xFF, dis >> 8 & 0xFF, dis >> 16 & 0xFF);
            };
            break;
        case 4:
            {
                dev = (max - min) / 4294967295.0;
                var dis = Math.round((value - min) / dev);
                buf.push(dis & 0xFF, dis >> 8 & 0xFF, dis >> 16 & 0xFF, dis >> 24 & 0xFF);
            };
            break;
        default:
            {
                dev_ = (max - min) / 255.0;
                var dis = Math.round((value - min) / dev);
                buf.push(dis & 0xFF);
            }
    };

}

// return
//{
//	pos: new_pos,
//	value:new_value 
//};
function i4things_get_discrete(buf, pos, min, max, container_size) {
    var dev;
    switch (container_size) {
        case 1:
            {
                dev = (max - min) / 255.0;
                var new_pos = pos + 1;
                var new_value = (min + (buf[pos] * dev));
                return {
                    pos: new_pos,
                    value: new_value
                };
            };
            break;
        case 2:
            {
                dev = (max - min) / 65535.0;
                var new_pos = pos + 2;
                var new_value = (min + ((buf[pos] + (buf[pos + 1] << 8)) * dev));
                return {
                    pos: new_pos,
                    value: new_value
                };
            };
            break;
        case 3:
            {
                dev = (max - min) / 16777215.0;
                var new_pos = pos + 3;
                var new_value = (min + ((buf[pos] + (buf[pos + 1] << 8) + (buf[pos + 2] << 16)) * dev));
                return {
                    pos: new_pos,
                    value: new_value
                };
            };
            break;
        case 4:
            {
                dev = (max - min) / 4294967295.0;
                var new_pos = pos + 3;
                var new_value = (min + ((buf[pos] + (buf[pos + 1] << 8) + (buf[pos + 2] << 16) + (buf[pos + 3] << 24)) * dev));
                    return {
                        pos: new_pos,
                        value: new_value
                    };
                };
                break;
                default: {
                    dev = (max - min) / 255.0;
                    var new_pos = pos + 1;
                    var new_value = (min + (buf[pos] * dev));
                    return {
                        pos: new_pos,
                        value: new_value
                    };
                }

            };

    }

    //add unsigned integer data up to 62bit ( 0 - 4611686018427387904) - the container will be adjusted automatically depending on the value
    //you need to ensure that you have at least 8 bytes available in the buffer - as this is the maximum bytes that the number can occupy in worst case scenario
    function i4things_add_uint(buf, value) {
        i4things_put_ots(value, buf);
    }

    // return
    //{
    //	pos: new_pos,
    //	value:new_value 
    //};
    function i4things_get_uint(buf, pos) {
        return i4things_get_ots(buf, pos);
    }
