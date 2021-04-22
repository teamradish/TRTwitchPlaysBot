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

#define MIN_AXIS_VAL -32767
#define MAX_AXIS_VAL 32767

int GetMinControllers();
int GetMaxControllers();

int GetMinAxisVal();
int GetMaxAxisVal();

int Emit(int fd, int eventType, int eventCode, int eventValue);

void UpdateJoystick(int fd);

void PressReleaseButton(int fd, int button, int press);

void PressAxis(int fd, int axis, int value);

int CreateController(int index);

void CloseController(int fd);
