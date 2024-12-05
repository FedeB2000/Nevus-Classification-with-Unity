# Nevus-Classification-with-Unity
Unity project using a deep learning ONNX model to classify images of skin lesions (nevi). The system analyzes 224x224x3 RGB images and provides as output the probabilities of belonging to the 'benign' or 'malignant' categories. This project has been implemented on an Android device with ARM 64-bit architecture.

# Requirement
1) Unity version: 6000.0.28f1 (tested on this version).
2) Barracuda Library: Barracuda GitHub Repository link: https://github.com/Unity-Technologies/barracuda-release.
3) NativeCameraUnity: NativeCamera GitHub Repository link: https://github.com/yasirkula/UnityNativeCamera.
4) Android Device with ARM 64-bit architecture.

# Installation
1) Clone this repository to your local machine.
2) Download Unity Hub link: https://unity.com/download.
3) Install Unity from UnityHub App if not already installed.
4) Download the required libraries and adding it to the Unity project.
5) Set up your Android build: In Unity, navigate to File > Build Settings and select Android as the target platform. Ensure that the Android device is set to ARM 64-bit for compatibility. Once everything is set up, build and run the project on your Android device.
   
# Usage
1) Launch the application.
2) Use the "LoadImage" button to take a photo from Device.
3) Use the "Predict" button, The image will be analyzed by the ONNX model, and the system will display the classification results as 'benign' or 'malignant' with respect probabilities.

# License
This project is licensed under the GNU AGPLv3 License. See the LICENSE file for details.
