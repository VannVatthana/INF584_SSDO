## Windows building
All relevant libraries are found in /libs and all DLLs found in /dlls (pre-)compiled for Windows. 
The CMake script knows where to find the libraries so just run CMake script to generate the project with commands below: 
    cd <path-to-project_INF584>
    mkdir build
    cd build
    cmake ..
Remark: Cmakelist does not contain the line for generating the executable. Therefore, I run the code from Visual Studio.
The mechanism of first persion shooting game is used to move around:
    - Press key W, A, S, D (for QWERTY keyboard) or Z, Q, S, D (for AZERTY) to go forward, right, backward and left, respectively
    - Use mouse to navigate the camera 
    - Use mouse scroll to zoom in and zoom out  
    - There is no direct key press in order to go up or down vertically,
        - The user has to rotate the camera toward the ceiling direction and press forward to go up, and backward to go down
        - and inversely, when the user rotates the camera toward the floor direction, press forward to go down and backward to go up