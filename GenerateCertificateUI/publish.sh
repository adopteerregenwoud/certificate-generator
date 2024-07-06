#!/bin/bash

# This script only works on my machine.
# TODO: change it so a release will be added to Github.

set -euo pipefail

APP_NAME=GenerateCertificateUI
DIST_DIR=./dist

if [ -d $DIST_DIR ]; then
    rm -rf $DIST_DIR
fi

TIMESTAMP=`date +'%Y-%m-%d_%H%M'`

echo "Adding timestamp to application..."
sed -i "s/__version_timestamp__/$TIMESTAMP/" MainWindow.axaml

echo "Running unit tests for $APP_NAME..."
dotnet test ../CertificateGeneratorCoreTests

echo "Building $APP_NAME..."
dotnet publish -c Release -r win-x64 \
   -p:PublishSingleFile=true -p:SelfContained=true \
   -p:IncludeAllContentForSelfExtract=true -p:TrimUnusedDependencies=true \
   --output $DIST_DIR

echo "Removing timestamp from source code..."
sed -i "s/$TIMESTAMP/__version_timestamp__/" MainWindow.axaml

echo "Deleting old zip files..."
rm -f /tmp/$APP_NAME*.zip

echo "Creating zip file..."
ZIP_NAME=${APP_NAME}-$TIMESTAMP.zip
ZIP_PATH=/tmp/$ZIP_NAME
cd $DIST_DIR && zip -r $ZIP_PATH *
cd -

echo "Uploading zip to fitlet..."
smbclient //fitlet/adopteerregenwoud -N -c "put /tmp/$ZIP_NAME $ZIP_NAME"

echo "Download URL = https://thuis.blauwe-lucht.nl/adopteer-regenwoud/$ZIP_NAME"
echo
echo "Done!"
