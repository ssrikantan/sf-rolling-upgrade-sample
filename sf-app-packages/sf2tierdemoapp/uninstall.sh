#!/bin/bash

sfctl application delete --application-id sf2tierdemoapp
sfctl application unprovision --application-type-name sf2tierdemoappType --application-type-version Version1.0
sfctl store delete --content-path sf2tierdemoapp
