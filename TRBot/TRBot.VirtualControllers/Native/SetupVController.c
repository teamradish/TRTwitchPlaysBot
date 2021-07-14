/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
 *
 * TRBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with TRBot.  If not, see <https://www.gnu.org/licenses/>.
*/

#include "SetupVController.h"

int GetMinControllers()
{
    return MIN_CONTROLLERS;
}

int GetMaxControllers()
{
    return MAX_CONTROLLERS;
}

int GetMinAxisVal()
{
    return MIN_AXIS_VAL;
}

int GetMaxAxisVal()
{
    return MAX_AXIS_VAL;
}

int Emit(int fd, int eventType, int eventCode, int eventValue)
{
    struct input_event ie;
    ie.type = eventType;
    ie.code = eventCode;
    ie.value = eventValue;
    
    return write(fd, &ie, sizeof(ie));
}

void UpdateJoystick(int fd)
{
    if (Emit(fd, EV_SYN, SYN_REPORT, 0) < 0)
    {
        printf("Error: UpdateJoystick() of descriptor %d", fd);
    }
}

void PressReleaseButton(int fd, int button, int press)
{
    if (Emit(fd, EV_KEY, button, press) < 0)
    {
        printf("Error: PressReleaseButton() | Button: %d | Press: %d", button, press);
    }
}

void PressAxis(int fd, int axis, int value)
{
    if (Emit(fd, EV_ABS, axis, value) < 0)
    {
        printf("Error: PressAxis() | Axis: %d | Value: %d", axis, value);
    }
}

int CreateController(int index)
{
    int fd = open("/dev/uinput", O_WRONLY | O_NONBLOCK); //opening of uinput
    if (fd < 0)
    {
        printf("Opening of uinput at index %d failed!\n", index);
        return INVALID_CONTROLLER;
    }
    
    printf("Opened uinput descriptor %d at index %d\n", fd, index);
    
    //Set up the gamepad buttons
    ioctl(fd, UI_SET_EVBIT, EV_KEY);
    ioctl(fd, UI_SET_KEYBIT, BTN_A);
    ioctl(fd, UI_SET_KEYBIT, BTN_B);
    ioctl(fd, UI_SET_KEYBIT, BTN_C);
    ioctl(fd, UI_SET_KEYBIT, BTN_X);
    ioctl(fd, UI_SET_KEYBIT, BTN_Y);
    ioctl(fd, UI_SET_KEYBIT, BTN_Z);
    ioctl(fd, UI_SET_KEYBIT, BTN_TL);
    ioctl(fd, UI_SET_KEYBIT, BTN_TR);
    ioctl(fd, UI_SET_KEYBIT, BTN_TL2);
    ioctl(fd, UI_SET_KEYBIT, BTN_TR2);
    ioctl(fd, UI_SET_KEYBIT, BTN_SELECT);
    ioctl(fd, UI_SET_KEYBIT, BTN_START);
    ioctl(fd, UI_SET_KEYBIT, BTN_MODE);
    ioctl(fd, UI_SET_KEYBIT, BTN_THUMBL);
    ioctl(fd, UI_SET_KEYBIT, BTN_THUMBR);
    ioctl(fd, UI_SET_KEYBIT, BTN_DPAD_UP);
    ioctl(fd, UI_SET_KEYBIT, BTN_DPAD_DOWN);
    ioctl(fd, UI_SET_KEYBIT, BTN_DPAD_LEFT);
    ioctl(fd, UI_SET_KEYBIT, BTN_DPAD_RIGHT);
    ioctl(fd, UI_SET_KEYBIT, BTN_0);
    ioctl(fd, UI_SET_KEYBIT, BTN_1);
    ioctl(fd, UI_SET_KEYBIT, BTN_2);
    ioctl(fd, UI_SET_KEYBIT, BTN_3);
    ioctl(fd, UI_SET_KEYBIT, BTN_4);
    ioctl(fd, UI_SET_KEYBIT, BTN_5);
    ioctl(fd, UI_SET_KEYBIT, BTN_6);
    ioctl(fd, UI_SET_KEYBIT, BTN_7);
    ioctl(fd, UI_SET_KEYBIT, BTN_8);
    ioctl(fd, UI_SET_KEYBIT, BTN_9);
    ioctl(fd, UI_SET_KEYBIT, BTN_JOYSTICK);
    ioctl(fd, UI_SET_KEYBIT, BTN_THUMB);
    ioctl(fd, UI_SET_KEYBIT, BTN_THUMB2);
    
    //Set up the gamepad axes - 8 in total
    ioctl(fd, UI_SET_EVBIT, EV_ABS);
    ioctl(fd, UI_SET_ABSBIT, ABS_X);
    ioctl(fd, UI_SET_ABSBIT, ABS_Y);
    ioctl(fd, UI_SET_ABSBIT, ABS_Z);
    ioctl(fd, UI_SET_ABSBIT, ABS_RX);
    ioctl(fd, UI_SET_ABSBIT, ABS_RY);
    ioctl(fd, UI_SET_ABSBIT, ABS_RZ);
    ioctl(fd, UI_SET_ABSBIT, ABS_HAT0X);
    ioctl(fd, UI_SET_ABSBIT, ABS_HAT0Y);
    
    struct uinput_user_dev uidev; //setting the default settings of Gamepad
    memset(&uidev, 0, sizeof(uidev));					   
    snprintf(uidev.name, UINPUT_MAX_NAME_SIZE, "TRBot Joypad %d", index); //Name of Gamepad
    
    uidev.id.bustype = BUS_USB;
    uidev.id.vendor  = 0x378 * (index + 1);
    uidev.id.product = 0x3;
    uidev.id.version = 2;
    uidev.absmax[ABS_X] = MAX_AXIS_VAL; //Parameters of thumbsticks
    uidev.absmin[ABS_X] = MIN_AXIS_VAL;
    uidev.absfuzz[ABS_X] = AXIS_FUZZ;
    uidev.absflat[ABS_X] = AXIS_FLAT;
    
    uidev.absmax[ABS_Y] = MAX_AXIS_VAL; 
    uidev.absmin[ABS_Y] = MIN_AXIS_VAL;
    uidev.absfuzz[ABS_Y] = AXIS_FUZZ;
    uidev.absflat[ABS_Y] = AXIS_FLAT;
    
    uidev.absmax[ABS_Z] = MAX_AXIS_VAL; 
    uidev.absmin[ABS_Z] = MIN_AXIS_VAL;
    uidev.absfuzz[ABS_Z] = AXIS_FUZZ;
    uidev.absflat[ABS_Z] = AXIS_FLAT;
    
    uidev.absmax[ABS_RX] = MAX_AXIS_VAL;
    uidev.absmin[ABS_RX] = MIN_AXIS_VAL;
    uidev.absfuzz[ABS_RX] = AXIS_FUZZ;
    uidev.absflat[ABS_RX] = AXIS_FLAT;
    
    uidev.absmax[ABS_RY] = MAX_AXIS_VAL;
    uidev.absmin[ABS_RY] = MIN_AXIS_VAL;
    uidev.absfuzz[ABS_RY] = AXIS_FUZZ;
    uidev.absflat[ABS_RY] = AXIS_FLAT;
    
    uidev.absmax[ABS_RZ] = MAX_AXIS_VAL; 
    uidev.absmin[ABS_RZ] = MIN_AXIS_VAL;
    uidev.absfuzz[ABS_RZ] = AXIS_FUZZ;
    uidev.absflat[ABS_RZ] = AXIS_FLAT;
    
    uidev.absmax[ABS_HAT0X] = MAX_AXIS_VAL; 
    uidev.absmin[ABS_HAT0X] = MIN_AXIS_VAL;
    uidev.absfuzz[ABS_HAT0X] = AXIS_FUZZ;
    uidev.absflat[ABS_HAT0X] = AXIS_FLAT;
    
    uidev.absmax[ABS_HAT0Y] = MAX_AXIS_VAL; 
    uidev.absmin[ABS_HAT0Y] = MIN_AXIS_VAL;
    uidev.absfuzz[ABS_HAT0Y] = AXIS_FUZZ;
    uidev.absflat[ABS_HAT0Y] = AXIS_FLAT;
    
    if(write(fd, &uidev, sizeof(uidev)) < 0) //writing settings
    {
        printf("error: write at index %d\n", index);
        return INVALID_CONTROLLER;
    }
    
    if(ioctl(fd, UI_DEV_CREATE) < 0) //writing ui dev create
    {
        printf("error: ui_dev_create at index %d\n", index);
        return INVALID_CONTROLLER;
    }
    
    return fd;
}

void CloseController(int fd)
{
    if (ioctl(fd, UI_DEV_DESTROY) < 0)
    {
        printf("Error destroying device with descriptor %d\n", fd);
        return;
    }
    
    close(fd);
}

//int main(int argc, char* argv[])
//{
//    int NumControllers = 1;
//    if (argc > 1)
//    {
//        NumControllers = atoi(argv[1]);
//    }
//
//    if (NumControllers < MIN_CONTROLLERS)
//    {
//        NumControllers = MIN_CONTROLLERS;
//        printf("Specified less than the min count of %d. Increasing to it\n", MIN_CONTROLLERS);
//    }
//    else if (NumControllers > MAX_CONTROLLERS)
//    {
//        NumControllers = MAX_CONTROLLERS;
//        printf("Specified more than max controller count of %d. Lowering to max count\n", MAX_CONTROLLERS);
//    }
//
//    printf("Starting application with %d controllers\n", NumControllers);
//    
//    int Descriptors[MAX_CONTROLLERS];
//    
//    for (int i = 0; i < NumControllers; i++)
//    {
//        Descriptors[i] = CreateController(i);
//    }
//    
//    char prev = 0;
//    while(1)
//    {
//        char c = getchar();
//        
//        if (c == prev)
//            continue;
//        
//        prev = c;
//        int descriptor = Descriptors[0];
//        
//        if (c == 'r')
//        {
//            PressReleaseButton(descriptor, BTN_A, 0);
//            UpdateJoystick(descriptor);
//        }
//        else if (c == 'p')
//        {
//            PressReleaseButton(descriptor, BTN_A, 1);
//            UpdateJoystick(descriptor);
//        }
//        else if (c == 'a')
//        {
//            PressAxis(descriptor, ABS_X, -32767);
//            UpdateJoystick(descriptor);
//        }
//        else if (c == 'w')
//        {
//            PressAxis(descriptor, ABS_Y, -32767);
//            UpdateJoystick(descriptor);
//        }
//        else if (c == 's')
//        {
//            PressAxis(descriptor, ABS_Y, 32767);
//            UpdateJoystick(descriptor);
//        }
//        else if (c == 'd')
//        {
//            PressAxis(descriptor, ABS_X, 32767);
//            UpdateJoystick(descriptor);
//        }
//        else if (c == 'z')
//        {
//            PressAxis(descriptor, ABS_X, 0);
//            UpdateJoystick(descriptor);
//        }
//        else if (c == 'x')
//        {
//            PressAxis(descriptor, ABS_Y, 0);
//            UpdateJoystick(descriptor);
//        }
//        else if (c == 'f')
//        {
//            PressAxis(descriptor, ABS_Z, 32767);
//            UpdateJoystick(descriptor);
//        }
//        else if (c == 'g')
//        {
//            PressAxis(descriptor, ABS_RZ, 32767);
//            UpdateJoystick(descriptor);
//        }
//        else if (c == 'v')
//        {
//            PressAxis(descriptor, ABS_Z, 0);
//            UpdateJoystick(descriptor);
//        }
//        else if (c == 'b')
//        {
//            PressAxis(descriptor, ABS_RZ, 0);
//            UpdateJoystick(descriptor);
//        }
//    }
//    
//    sleep(1);
//    
//    for (int i = 0; i < NumControllers; i++)
//    {
//        int val = Descriptors[i];
//        CloseController(Descriptors[i]);
//        
//        printf("Descriptor at %d was %d and is now %d\n", i, val, Descriptors[i]);
//    }
//    
//    return 0;
//}
