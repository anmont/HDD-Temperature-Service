HDD-Temperature-Service
=======================

Windows HDD Temperature Monitor Service

_________________________________________
This application runs as a service.
There is no GUI.
_________________________________________

_________________________________________
Requirements
HDD SMART info is only exposed to users
with System Management access. This is an
administrator normally. The service will
not make any alerts if your user does
not have sufficient priveleges.
_________________________________________



An installer functions to install/uninstall the service.

HDD Temperature Notifications:

Warnings occur at 46 degrees celcius
- a distinct series of two beeps occur every 10 seconds to let you know a hard disk has exceeded this value

Alarms occur at 50 degrees
- a distinct series of three beeps occur every 7 seconds to let you know a hard disk has exceeded this value

