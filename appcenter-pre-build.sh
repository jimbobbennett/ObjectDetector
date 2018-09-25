#!/usr/bin/env bash
#
# For Xamarin, change some constants located in some class of the app.
# In this sample, suppose we have an AppConstant.cs class in shared folder with follow content:
#
# namespace Core
# {
#     public class AppConstant
#     {
#         public const string ApiUrl = "https://production.com/api";
#     }
# }
# 
# Suppose in our project exists two branches: master and develop. 
# We can release app for production API in master branch and app for test API in develop branch. 
# We just need configure this behaviour with environment variable in each branch :)
# 
# The same thing can be perform with any class of the app.
#
# AN IMPORTANT THING: FOR THIS SAMPLE YOU NEED DECLARE API_URL ENVIRONMENT VARIABLE IN APP CENTER BUILD CONFIGURATION.

if [ ! -n "$APP_CENTER_SETTING" ]
then
    echo "You need define the APP_CENTER_SETTING variable in App Center"
    exit
fi

APP_CONSTANT_FILE=$APPCENTER_SOURCE_DIRECTORY/ObjectDetector/AppConstants.cs

if [ -e "$APP_CONSTANT_FILE" ]
then
    echo "Updating AppCenterKey to $APP_CENTER_SETTING in AppConstants.cs"
    sed -i '' 's#AppCenterKey = "ios="#AppCenterKey = "ios='$APP_CENTER_SETTING'"#' $APP_CONSTANT_FILE

    echo "File content:"
    cat $APP_CONSTANT_FILE
fi

INFO_PLIST_FILE=$APPCENTER_SOURCE_DIRECTORY/ObjectDetector.iOS/Info.plist

if [ -e "$INFO_PLIST_FILE" ]
then
    echo "Updating AppCenterKey to $APP_CENTER_SETTING in Info.plist"
    sed -i '' 's#appcenter-distributionkey#appcenter-'$APP_CENTER_SETTING'#' $INFO_PLIST_FILE

    echo "File content:"
    cat $INFO_PLIST_FILE
fi

GOOGLE_JSON_FILE=$APPCENTER_SOURCE_DIRECTORY/ObjectDetector.Android/google-services.json

if [ -e "$GOOGLE_JSON_FILE" ]
then
    echo "Updating Google Json"
    echo "$GOOGLE_JSON" > $GOOGLE_JSON_FILE
    sed -i -e 's/\\"/'\"'/g' $GOOGLE_JSON_FILE

    echo "File content:"
    cat $GOOGLE_JSON_FILE
fi


