#Display/Consume API

Following is a actual example how request data for the day and send data to a node - including  decrypt , iterate and display data in a HTML page with simple javascript.
 
IMPORTANT
 
When node/thing send data - if the data is not able to be received/acknowledge from available gateway for 1 minute - the data will be discarded. During this minute the sending  will be retried multiple times. The user can subscribe to receive callback if the data is discarded/timeout or  acknowledged.
 
When data is scheduled to be delivered to the node/thing then the server will throttle the requests once per minute to the last gateway to which the node/thing has sent successfully data.  This in effect means that the minimum time in which the data can be delivered to the node/thing is 1 minute.  In case the gateway is too busy ( too many messages are scheduled to be delivered to end nodes/things - after 1/2h waiting to be delivered the data will be discarded.
 
Data returned is in the following format:
```
var iot_json = ' {
    "thing": 1000, // device ID
    "last": [{  // list of update points for the day
        "t": 1543410612789, // UTC time in milliseconds from 01/01/1970 ( unix format)
        "l": 42.66774, // triangulated latitude 
        "n": 23.304007, // triangulated longitude
        "r": 81, // maximum signal strength to the closest gateway in %
        "d": [207, 248, 239, 233, 114, 87, 4, 136] // encrypted node data 
    }, {
        "t": 1543409791252,
        "l": 42.667773,
        "n": 23.303743,
        "r": 82,
        "d": [207, 248, 239, 233, 114, 87, 4, 136]
    }]}';
    
```

Actual source code :

```

<html>
<body>

<p><div id="iot_data_send"></div></p>

<p><div id="iot_data_dump"></div></p>


<script src="https://i4things.com/assets/i4t/js/i4things.js"></script>


<script>	
/**********************************************************\
				  Actual code here - make sure you have the 
				  same key as in the IoT device
\**********************************************************/



	
function iot_json_function(data, key) {
        var json_data = JSON.parse(data);
		
		 /**********************************************************\
					Place your code here
         \**********************************************************/
		
        var out = "Thing: " + json_data.thing + "<br>";
        for (i = 0; i < json_data.last.length; i++) {
		  out += "<hr><p>";
          out += "&nbsp;Time: " + new Date(json_data.last[i].t) + "<br>";
          out += "&nbsp;Signal Strength: " + json_data.last[i].r + "%<br>";
		  out += "&nbsp;Triangulated Lat: " + json_data.last[i].l + "<br>";
		  out += "&nbsp;Triangulated Lon: " + json_data.last[i].n + "<br>";
          out += "&nbsp;Data: "
		  
		  var decrypted_data = xxtea_decrypt(json_data.last[i].d, key);
          
		  for (j = 0; j < decrypted_data.length; j++) {
             out +=  decrypted_data[j] + "";
          }
		  
          out += "<br>";
		  
		  //custom example
		  out += "<br>";
		  var degree_celsius = (-20.0 + (decrypted_data[0] * 0.3137)).toFixed(1); out += "&nbsp;Temp: " + degree_celsius +"C<br>";
          var humidity_percent = decrypted_data[1]; out += "&nbsp;Humidity: " + humidity_percent +"%<br>";
		  var bat_voltage = (2.0 + (decrypted_data[2] * 0.0118)).toFixed(2); out += "&nbsp;Battery: " + bat_voltage +"V<br>";
		  
		  out += "<\p>";
		  
        }
 document.getElementById("iot_data_dump").innerHTML = out;
}

</script>

<script> 
/**********************************************************\
				  Can be called from event
\**********************************************************/

var thing_id = 1;
var thing_network_key = "4C11182A1D152D5D0F62144A44523C25";
var thing_private_key = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
var message_to_node = [1,2,3, Math.floor((Math.random() * 255))];
		 
// get todays data		 
         i4things_load_script("http://server.i4things.com:5408/iot_get/" + i4things_get_data_request(thing_id, thing_network_key) , function() {
		 /**********************************************************\
					The Server will return : var iot_json = '{....}';
         \**********************************************************/
			iot_json_function(iot_json, thing_private_key);
		 });
		
		
// send data to device		 
         i4things_load_script("http://server.i4things.com:5408/iot_set/" + i4things_set_data_request(thing_id, message_to_node, thing_network_key, thing_private_key) , function() {
		 /**********************************************************\
					The Server will return : 
					if ALL OK :
					iot_json = '{ "RES" : "OK" }';
					if ERROR :
					var iot_json = '{ "ERR" : "message too big" }';

         \**********************************************************/
			if (iot_json.indexOf("OK") !== -1)
			{
			  document.getElementById("iot_data_send").innerHTML = "DATA SENT SUCCESSFULLY!";
			}
			else
			{
			  document.getElementById("iot_data_send").innerHTML = "DATA SENT ERROR!";
			}
		 });		 
		 
</script>

</body>
</html>

```

 

PRACTICALITY AND EFFICIENCY

In the API there are four very helpful functions related to easy and efficient way of representing data in byte buffer(byte array) message :

```

function i4things_add_discrete(buf, min, max, value, container_size)
function i4things_get_discrete(buf, pos, min, max, container_size)

function i4things_add_uint(buf, value)
function i4things_get_uint(buf, pos)

```

The result from the get fuctions is a object with following sctructure :

```

return { pos: 1, // new pos
         value: 23.5  // value
       };
```

Using the add/get discrete you can add to the buffer and read from the buffer discrete values - e.g. if you want to store temperate in the interval between -20C and +60C efficiently in only one byte but still having better resolution then 1/2 degree then you can use the discrete functions in the following fashion:

```
var buf = [];

var temp1 = 23.5;
var temp1 = 18.1;

i4things_add_discrete(buf, -20, 60, 23.5, 1);
i4things_add_discrete(buf, -20, 60, 18.1, 1);

var re1 = i4things_get_discrete(buf, 0, -20, 60, 1);
var temp1_read_from_buffer = ret1.value;
var pos1 = ret1.pos;
 
var ret2 = i4things_get_discrete(buf, pos1, -20, 60, 1); 
var temp2_read_from_buffer = ret2.value;
```

Using the add/get uint you can add to the buffer and read from the buffer positive integer values between 0 and 4611686018427387904. The value will be stored in the minimum possible bytes e.g. if the value can fit in one byte it will occupy only one byte, if it can fit in 2 bytes it will occupy only two bytes and etc. up to 8 bytes. This way you can save space in the message, be efficient and optimize your traffic:

```
var buf = [];

//insert into the buffer
i4things_add_uint(buf, 11); // will occupy only 1 byte
i4things_add_uint(buf, 500); // will occupy only 2 bytes

var re1 = i4things_get_uint(buf, 0);
var val1_read_from_buffer = ret1.value;
var pos1 = ret1.pos;
 
var ret2 = i4things_get_uint(buf, pos1); 
var val2_read_from_buffer = ret2.value;
```
