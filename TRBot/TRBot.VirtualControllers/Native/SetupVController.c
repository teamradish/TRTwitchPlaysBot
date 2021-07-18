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

void UpdateJoystick(struct libholder* libholder)
{
    if (libevdev_uinput_write_event(libholder->uidev, EV_SYN, SYN_REPORT, 0) < 0)
    {
        printf("Error: UpdateJoystick() of descriptor %d\n", libevdev_get_fd(libholder->dev));
    }
}

void PressReleaseButton(struct libholder* libholder, int button, int press)
{
    if (libevdev_uinput_write_event(libholder->uidev, EV_KEY, button, press) < 0)
    {
        printf("Error: PressReleaseButton() | Button: %d | Press: %d\n", button, press);
    }
}

void PressAxis(struct libholder* libholder, int axis, int value)
{
    if (libevdev_uinput_write_event(libholder->uidev, EV_ABS, axis, value) < 0)
    {
        printf("Error: PressAxis() | Axis: %d | Value: %d\n", axis, value);
    }
}

struct input_absinfo* CreateAbsInfo()
{
    struct input_absinfo* absInfo = malloc(sizeof(*absInfo));

    absInfo->minimum = MIN_AXIS_VAL;
    absInfo->maximum = MAX_AXIS_VAL;
    absInfo->fuzz = AXIS_FUZZ;
    absInfo->flat = AXIS_FLAT;
    absInfo->resolution = 0;

    return absInfo;
}

struct libholder* CreateController(int index)
{
    struct libevdev* dev = libevdev_new();

    //Set the name of the controller
    char name[UINPUT_MAX_NAME_SIZE];
    snprintf(name, UINPUT_MAX_NAME_SIZE, "TRBot Joypad %d", index);

    libevdev_set_name(dev, name);

    //Buttons
    libevdev_enable_event_type(dev, EV_KEY);
    libevdev_enable_event_code(dev, EV_KEY, BTN_A, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_B, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_C, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_X, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_Y, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_Z, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_TL, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_TR, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_TL2, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_TR2, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_SELECT, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_START, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_MODE, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_THUMBL, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_THUMBR, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_DPAD_UP, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_DPAD_DOWN, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_DPAD_LEFT, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_DPAD_RIGHT, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_0, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_1, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_2, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_3, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_4, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_5, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_6, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_7, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_8, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_9, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_JOYSTICK, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_THUMB, NULL);
    libevdev_enable_event_code(dev, EV_KEY, BTN_THUMB2, NULL);

    //Set up the gamepad axes - 8 in total
    libevdev_enable_event_type(dev, EV_ABS);
    
    //Only one input_absinfo pointer is necessary since libevdev just uses values from it
    //This means it can be discarded afterwards
    struct input_absinfo* absInfo = CreateAbsInfo();

    libevdev_enable_event_code(dev, EV_ABS, ABS_X, absInfo);
    libevdev_enable_event_code(dev, EV_ABS, ABS_Y, absInfo);
    libevdev_enable_event_code(dev, EV_ABS, ABS_Z, absInfo);
    libevdev_enable_event_code(dev, EV_ABS, ABS_RX, absInfo);
    libevdev_enable_event_code(dev, EV_ABS, ABS_RY, absInfo);
    libevdev_enable_event_code(dev, EV_ABS, ABS_RZ, absInfo);
    libevdev_enable_event_code(dev, EV_ABS, ABS_HAT0X, absInfo);
    libevdev_enable_event_code(dev, EV_ABS, ABS_HAT0Y, absInfo);

    //Set device information
    libevdev_set_id_bustype(dev, BUS_USB);
    libevdev_set_id_vendor(dev, 0x378 * (index + 1));
    libevdev_set_id_product(dev, 0x3);
    libevdev_set_id_version(dev, 3);

    //Free the input_absinfo pointer since we no longer need it
    free(absInfo);

    //Set up the uidev device
    struct libevdev_uinput* uidev = NULL;

    //Creating with LIBEVDEV_UINPUT_OPEN_MANAGED means that libevdev will manage the uinput file descriptor
    int err = libevdev_uinput_create_from_device(dev, LIBEVDEV_UINPUT_OPEN_MANAGED, &uidev);
    if (err != 0)
    {
        return INVALID_CONTROLLER;
    }
    
    //Both the libevdev and libevdev_uinput pointers are necessary, so store them in a new object and return that
    struct libholder *libholder = malloc(sizeof(*libholder));
    libholder->dev = dev;
    libholder->uidev = uidev;
    
    return libholder;
}

void CloseController(struct libholder* libholder)
{
    //Free the libevdev_uinput pointer followed by the libevdev pointer
    //The uinput file descriptor will be automatically closed since the device was created with LIBEVDEV_UINPUT_OPEN_MANAGED
    libevdev_uinput_destroy(libholder->uidev);
    libevdev_free(libholder->dev);

    //Finally, free the container object
    free(libholder);
}

/*int main(int argc, char* argv[])
{
    int NumControllers = 1;
    if (argc > 1)
    {
        NumControllers = atoi(argv[1]);
    }

    if (NumControllers < MIN_CONTROLLERS)
    {
        NumControllers = MIN_CONTROLLERS;
        printf("Specified less than the min count of %d. Increasing to it\n", MIN_CONTROLLERS);
    }
    else if (NumControllers > MAX_CONTROLLERS)
    {
        NumControllers = MAX_CONTROLLERS;
        printf("Specified more than max controller count of %d. Lowering to max count\n", MAX_CONTROLLERS);
    }

    printf("Starting application with %d controllers\n", NumControllers);
    
    struct libholder* Descriptors[MAX_CONTROLLERS];
    
    for (int i = 0; i < NumControllers; i++)
    {
        Descriptors[i] = CreateController(i);
    }
    
    char prev = 0;
    while(1)
    {
        char c = getchar();
        
        if (c == prev)
            continue;
        
        prev = c;
        struct libholder* descriptor = Descriptors[0];
        
        if (c == 'r')
        {
            PressReleaseButton(descriptor, BTN_A, 0);
            UpdateJoystick(descriptor);
        }
        else if (c == 'p')
        {
            PressReleaseButton(descriptor, BTN_A, 1);
            UpdateJoystick(descriptor);
        }
        else if (c == 'a')
        {
            PressAxis(descriptor, ABS_X, MIN_AXIS_VAL);
            UpdateJoystick(descriptor);
        }
        else if (c == 'w')
        {
            PressAxis(descriptor, ABS_Y, MIN_AXIS_VAL);
            UpdateJoystick(descriptor);
        }
        else if (c == 's')
        {
            PressAxis(descriptor, ABS_Y, MAX_AXIS_VAL);
            UpdateJoystick(descriptor);
        }
        else if (c == 'd')
        {
            PressAxis(descriptor, ABS_X, MAX_AXIS_VAL);
            UpdateJoystick(descriptor);
        }
        else if (c == 'z')
        {
            PressAxis(descriptor, ABS_X, 0);
            UpdateJoystick(descriptor);
        }
        else if (c == 'x')
        {
            PressAxis(descriptor, ABS_Y, 0);
            UpdateJoystick(descriptor);
        }
        else if (c == 'f')
        {
            PressAxis(descriptor, ABS_Z, MAX_AXIS_VAL);
            UpdateJoystick(descriptor);
        }
        else if (c == 'g')
        {
            PressAxis(descriptor, ABS_RZ, MAX_AXIS_VAL);
            UpdateJoystick(descriptor);
        }
        else if (c == 'v')
        {
            PressAxis(descriptor, ABS_Z, 0);
            UpdateJoystick(descriptor);
        }
        else if (c == 'b')
        {
            PressAxis(descriptor, ABS_RZ, 0);
            UpdateJoystick(descriptor);
        }
    }
    
    sleep(1);
    
    for (int i = 0; i < NumControllers; i++)
    {
        struct libholder* val = Descriptors[i];

        int descriptor = libevdev_get_fd(val->dev);

        CloseController(Descriptors[i]);
        
        printf("Descriptor at %d was %d and is now closed\n", i, descriptor);
    }
    
    return 0;
}*/
