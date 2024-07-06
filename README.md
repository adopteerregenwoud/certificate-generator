# Generate certificate

Tool that generates [adopteerregenwoud.nl](https://www.adopteerregenwoud.nl/) / [adoptrainforest.com](https://www.adoptrainforest.com/) certificates.

This repository only contains the code to generate the certificates.
The certificate templates are proprietary and therefore not included. However you can create your own templates.

Note that almost everything is hard-coded for Adopteer Regenwoud certificates.
To use this code for your own certificates, either fork and change or create PRs for making the tools more data-driven.

## Certificate templates requirements

For the tools to function properly a directory is required that contains a number of templates:

- 1-dutch.png
- 1-english.png
- 4-dutch.png
- 4-english.png
- 10-dutch.png
- 10-english.png
- 20-dutch.png
- 20-english.png
- 50-dutch.png
- 50-english.png
- 100-dutch.png
- 100-english.png

The number represents for which amount of m² the certificate is used. The language should be obvious.

Each template should be exactly 3507 x 2480 pixels.

## How to use the tools

### GenerateCertificateUI

Build or download GenerateCertificateUI.exe.

- Create a new folder for the application, e.g. C:\GenerateCertificate.
- Move the .exe file into that folder.
- Put the template files in C:\GenerateCertificate\templates.
- Create a new folder where the certificates will be stored, e.g., C:\GenerateCertificate\certificates.
- Start the application.
- In 'Template folder:', enter the folder where the templates are located, C:\GenerateCertificate\templates.
- In 'Output folder:', enter the folder where the certificates will be stored, C:\GenerateCertificate\certificates.

You only need to do this once. Once you have successfully generated a certificate, it will remember the folders you entered.

After this, you can enter the information for a certificate and click 'Generate Certificate'.
It will then start working, and when it is done, it will show you where it has placed the file.
You can then create more certificates by adjusting the entered information and clicking 'Generate Certificate' again,
you don't need to restart the application each time.
