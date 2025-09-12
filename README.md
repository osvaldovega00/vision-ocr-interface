# Vision-Based OCR Reading System

**Summary:**

This OCR Reading Application is a Windows-based interface designed to perform optical character recognition on lead markings using a Cognex In-Sight industrial vision camera. The application supports automated serial number extraction for various lead types, featuring real-time camera control, configurable settings, and robust logging and troubleshooting tools.

**Project Structure:**

*   ocr-reading.sln -> Visual Studio solution file
*   ocr-reading.exe -> Executable application
*   InputHelper.dll -> DLL for keyboard hook operations
*   OCR Vision Reading Settings.xml -> Configuration for camera and app parameters
*   ocr-reading.job -> Cognext In-Sight job file for OCR tools
*   Cognex_InSight_SDK_Installer.exe -> SDK for Cognex In-Sight camera communication
    *  This application requires the Cognex In-Sight SDK, download using link below.   
    *  Download Link - https://support.cognex.com/en/downloads/detail/in-sight/4532/1033

**System Requirements:**

*   Windows 10+ (64 bit)
*   Cognex In-Sight Explorer V5.7.4
*   Cognex In-Sight camera
*   External light controller (e.g. Gardasoft PP520)


**Setup Steps:**

1.   Copy the folder named "Vision-Based OCR Reading System Project" to the root of your system drive
2.   Navigate to: "C:\Vision-Based OCR Reading System Project\Vision-Based OCR Reading System\NeuroModulation OCR Reading\bin\Release"
3.   Right-click "ocr-reading.exe", then "Create Shortcut"
4.   Place the shortcut on your Desktop or preferred launch location
5.   A sanitized template named "OCR Vision Reading Settings Template.xml" has been provided, to use it navigate to "C:\Vision-Based OCR Reading System Project\Settings", rename file to "OCR Vision Reading Settings.xml" and adjust as needed.
6.   Ensure the Cognex In-Sight camera is connected and configured via ethernet
7.   Launch the application using the shortcut


**Technologies Used:**

*   VB / .NET Framework -> UI and application logic
*   Cognex In-Sight SDK -> Camera communication libraries
*   XML Configuration -> Dynamic app parameter control
*   Cognex In-Sight Job File -> OCR and focus tools for vision system
*   Windows Forms -> GUI development
*   DLL Integration -> Keyboard hook functionality

**App Preview:**

<img width="975" height="516" alt="image" src="https://github.com/user-attachments/assets/1b374f40-cb1f-4573-af22-923874f843b0" />
