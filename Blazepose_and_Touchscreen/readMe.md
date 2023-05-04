Final code repository for the pull gestures project.

Fisheye unwarping contains all the code for unwarping the arducam fisheye lens.
  - Bot/top_calibration contains all the code for calibrating the bot/top camera. 
  - firstround is DEPRECATED, just contains the frames from the first try at calibrating.
  - possible.txt (and possible_bot / possible_top) has possible unwarping parameters.

Camera: [link](https://www.amazon.com/Arducam-Computer-Fisheye-Microphone-Windows/dp/B07ZS75KZR/ref=sr_1_5?crid=2JR9OSMEE0D41&keywords=wide+angle+arducam+usb&qid=1642540801&sprefix=wide+angle+arducam+usb%2Caps%2C67&sr=8-5)
Lens: [link](https://www.amazon.com/dp/B013JWEGJQ?psc=1&ref=ppx_yo2_dt_b_product_details)

pixel_to_realworld contains the code for mapping pixel coordinates to realworld values.
  - measure_xy / z are for manually getting the interpolation steps from measurements (MUST set up rulers)
  - try_x/y/depth are for actually visualizing the interpolation
  - onecam_numbers.txt is DEPRECATED, sample numbers before actually cementing our hardware.

pull_guesture_demos contains all the unity stuff for all the video demos!

2cam_socket.py is the proxy server to run to transmit hand data to unity
  - bp_threaded_socket is the same thing, but with just one camera
  - try_bp is just trying to run blazepose
  - try_bp_2cam is just trying to run blazepose on two cameras
