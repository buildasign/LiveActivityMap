#Live Activity Map
##Overview
The Live Activity Map is an "Eye Candy" display of a globe that zooms into locations in real time as location data is provided. 
This web application is best suited for a highly active event such as sales.

The application was started in the Dev Lab at BuildASign.com. It is a mashup of [Google Earth] (https://developers.google.com/earth/documentation/), 
the [GeoNames geocoding API] (http://www.geonames.org/export/web-services.html), 
and [SignalR] (http://signalr.net/). The event's city, state, and postal code are supplied to the SignalR hub 
on the Live Activity Map web application. The location data is geocoded to get the latitude and longitude. 
The lat/long is then sent via SignalR to all of the clients so Google Earth can display the new location.

##Configuration
To run this application, you will need an api username from GeoNames. This must be supplied in the web.config file.

##Demo
A demonstration is provided in the SampleActivityNotifier console application. 
It plays back a list of sales events in real time to demonstrate how a source application can communicate with the Live Activity Map.