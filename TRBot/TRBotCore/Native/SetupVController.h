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
#define INVALID_CONTROLLER -1

int GetMinControllers();
int GetMaxControllers();

int Emit(int fd, int eventType, int eventCode, int eventValue);

void UpdateJoystick(int fd);

void PressReleaseButton(int fd, int button, int press);

void PressAxis(int fd, int axis, int value);

int CreateController(int index);

void CloseController(int fd);
