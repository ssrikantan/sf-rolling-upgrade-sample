#!/bin/bash

sfctl application upload --path sf2tierdemoapp --show-progress
sfctl application provision --application-type-build-path sf2tierdemoapp
sfctl application create --app-name fabric:/sf2tierdemoapp --app-type sf2tierdemoappType --app-version Version1.0
