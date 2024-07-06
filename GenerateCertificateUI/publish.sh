#!/bin/bash

# This script only works on my machine.
# TODO: change it so a release will be added to Github.

set -euo pipefail

APP_NAME=GenerateCertificateUI
DIST_DIR=./dist

rawurlencode() {
  local string="${1}"
  local strlen=${#string}
  local encoded=""
  local pos c o

  for (( pos=0 ; pos<strlen ; pos++ )); do
     c=${string:$pos:1}
     case "$c" in
        [-_.~a-zA-Z0-9] ) o="${c}" ;;
        * )               printf -v o '%%%02x' "'$c"
     esac
     encoded+="${o}"
  done
  echo "${encoded}"    # You can either set a return variable (FASTER) 
  REPLY="${encoded}"   #+or echo the result (EASIER)... or both... :p
}

if [ -d $DIST_DIR ]; then
    rm -rf $DIST_DIR
fi

echo "Running unit tests for $APP_NAME..."
dotnet test ../CertificateGeneratorCoreTests

echo "Building $APP_NAME..."
dotnet publish -c Release -r win-x64 \
   -p:PublishSingleFile=true -p:SelfContained=true \
   -p:IncludeAllContentForSelfExtract=true -p:TrimUnusedDependencies=true \
   --output $DIST_DIR

echo "Deleting old zip files..."
rm -f /tmp/$APP_NAME*.zip

echo "Creating zip file..."
TIMESTAMP=`date +'%Y-%m-%d_%H%M'`
ZIP_NAME=${APP_NAME}-$TIMESTAMP.zip
ZIP_PATH=/tmp/$ZIP_NAME
cd $DIST_DIR && zip -r $ZIP_PATH *
cd -

echo "Uploading zip to fitlet..."
smbclient //fitlet/adopteerregenwoud -N -c "put /tmp/$ZIP_NAME $ZIP_NAME"

echo "Download URL = https://thuis.blauwe-lucht.nl/adopteer-regenwoud/$ZIP_NAME"
echo
echo "Done!"
