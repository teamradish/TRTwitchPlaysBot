#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <fcntl.h>
#include <errno.h>
#include <linux/input.h>
#include <linux/uinput.h>

#define MIN_CONTROLLERS 1
#define MAX_CONTROLLERS 16

int NumControllers = 1;
int Descriptors[MAX_CONTROLLERS];   

int Emit(int fd, int eventType, int eventCode, int eventValue)
{
    struct input_event ie;
    ie.type = eventType;
    ie.code = eventCode;
    ie.value = eventValue;
    
    return write(fd, &ie, sizeof(ie));
}

void UpdateJoystick(int index)
{
    if (Emit(Descriptors[index], EV_SYN, SYN_REPORT, 0) < 0)
    {
        printf("Error: UpdateJoystick()");
    }
}

void PressReleaseButton(int index, int button, int press)
{
    if (Emit(Descriptors[index], EV_KEY, button, press) < 0)
    {
        printf("Error: PressReleaseButton() | Button: %d | Press: %d", button, press);
    }
}

void PressAxis(int index, int axis, int value)
{
    if (Emit(Descriptors[index], EV_ABS, axis, value) < 0)
    {
        printf("Error: PressAxis() | Axis: %d | Value: %d", axis, value);
    }
}

void SetupController(int index)
{
    int fd = open("/dev/uinput", O_WRONLY | O_NONBLOCK); //opening of uinput
    if (fd < 0)
    {
        printf("Opening of uinput at index %d failed!\n", index);
        return;
    }
    
    Descriptors[index] = fd;
    
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
    
    //Set up the gamepad axes - 6 in total
    ioctl(fd, UI_SET_EVBIT, EV_ABS);
    ioctl(fd, UI_SET_ABSBIT, ABS_X);
    ioctl(fd, UI_SET_ABSBIT, ABS_Y);
    ioctl(fd, UI_SET_ABSBIT, ABS_Z);
    ioctl(fd, UI_SET_ABSBIT, ABS_RX);
    ioctl(fd, UI_SET_ABSBIT, ABS_RY);
    ioctl(fd, UI_SET_ABSBIT, ABS_RZ);
    
    struct uinput_user_dev uidev; //setting the default settings of Gamepad
    memset(&uidev, 0, sizeof(uidev));					   
    snprintf(uidev.name, UINPUT_MAX_NAME_SIZE, "TRBot Joypad %d", index); //Name of Gamepad
    
    uidev.id.bustype = BUS_USB;
    uidev.id.vendor  = 0x3 * (index + 1);
    uidev.id.product = 0x3;
    uidev.id.version = 2;
    uidev.absmax[ABS_X] = 32767; //Parameters of thumbsticks
    uidev.absmin[ABS_X] = -32767;
    uidev.absfuzz[ABS_X] = 0;
    uidev.absflat[ABS_X] = 15;
    
    uidev.absmax[ABS_Y] = 32767; 
    uidev.absmin[ABS_Y] = -32767;
    uidev.absfuzz[ABS_Y] = 0;
    uidev.absflat[ABS_Y] = 15;
    
    uidev.absmax[ABS_Z] = 32767; 
    uidev.absmin[ABS_Z] = -32767;
    uidev.absfuzz[ABS_Z] = 0;
    uidev.absflat[ABS_Z] = 15;
    
    uidev.absmax[ABS_RX] = 32767;
    uidev.absmin[ABS_RX] = -32767;
    uidev.absfuzz[ABS_RX] = 0;
    uidev.absflat[ABS_RX] = 16;
    
    uidev.absmax[ABS_RY] = 32767;
    uidev.absmin[ABS_RY] = -32767;
    uidev.absfuzz[ABS_RY] = 0;
    uidev.absflat[ABS_RY] = 16;
    
    uidev.absmax[ABS_RZ] = 32767; 
    uidev.absmin[ABS_RZ] = -32767;
    uidev.absfuzz[ABS_RZ] = 0;
    uidev.absflat[ABS_RZ] = 16;
    
    if(write(fd, &uidev, sizeof(uidev)) < 0) //writing settings
    {
        printf("error: write at index %d\n", index);
        return;
    }
    if(ioctl(fd, UI_DEV_CREATE) < 0) //writing ui dev create
    {
        printf("error: ui_dev_create at index %d\n", index);
        return;
    }
}

int main(int argc, char* argv[])
{
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
    
    for (int i = 0; i < NumControllers; i++)
    {
        SetupController(i);
    }
    
    char prev = 0;
    while(1)
    {
        char c = getchar();
        
        if (c == prev)
            continue;
        
        prev = c;
        
        if (c == 'r')
        {
            PressReleaseButton(0, BTN_A, 0);
            UpdateJoystick(0);
        }
        else if (c == 'p')
        {
            PressReleaseButton(0, BTN_A, 1);
            UpdateJoystick(0);
        }
        else if (c == 'a')
        {
            PressAxis(0, ABS_X, -32767);
            UpdateJoystick(0);
        }
        else if (c == 'w')
        {
            PressAxis(0, ABS_Y, -32767);
            UpdateJoystick(0);
        }
        else if (c == 's')
        {
            PressAxis(0, ABS_Y, 32767);
            UpdateJoystick(0);
        }
        else if (c == 'd')
        {
            PressAxis(0, ABS_X, 32767);
            UpdateJoystick(0);
        }
        else if (c == 'z')
        {
            PressAxis(0, ABS_X, 0);
            UpdateJoystick(0);
        }
        else if (c == 'x')
        {
            PressAxis(0, ABS_Y, 0);
            UpdateJoystick(0);
        }
        else if (c == 'f')
        {
            PressAxis(0, ABS_Z, 32767);
            UpdateJoystick(0);
        }
        else if (c == 'g')
        {
            PressAxis(0, ABS_RZ, 32767);
            UpdateJoystick(0);
        }
        else if (c == 'v')
        {
            PressAxis(0, ABS_Z, 0);
            UpdateJoystick(0);
        }
        else if (c == 'b')
        {
            PressAxis(0, ABS_RZ, 0);
            UpdateJoystick(0);
        }
        
        /*continue;
        memset(&ev, 0, sizeof(struct input_event)); //setting the memory for event
        ev.type = EV_KEY;
        ev.code = BTN_X;
        ev.value = !toggle;
        toggle = !toggle;
        if(write(fd, &ev, sizeof(struct input_event)) < 0) //writing the key change
        {
            printf("error: key-write");
            return 1;
        }
        memset(&ev, 0, sizeof(struct input_event)); //setting the memory for event
        ev.type = EV_ABS;
        ev.code = ABS_X;
        ev.value = toggle == 1 ? 256 : 0;
        if(write(fd, &ev, sizeof(struct input_event)) < 0) //writing the thumbstick change
        {
            printf("error: thumbstick-write");
            return 1;
        }
        memset(&ev, 0, sizeof(struct input_event));
        ev.type = EV_SYN;
        ev.code = SYN_REPORT;
        ev.value = 0;
        if(write(fd, &ev, sizeof(struct input_event)) < 0) //writing the sync report
        {
            printf("error: sync-report");
            return 1;
        }*/
    }
    
    for (int i = 0; i < NumControllers; i++)
    {
        if (ioctl(Descriptors[i], UI_DEV_DESTROY) < 0)
        {
            printf("Error destroying device at index %d\n", i);
            continue;
        } 
        
        close(Descriptors[i]);
    }
    
    return 0;
}
