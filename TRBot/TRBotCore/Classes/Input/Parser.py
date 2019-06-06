import re
from re import match



# SNES

VALID_INPUTS = [
	"left", "right", "up", "down",
	"a", "b", "l", "r", "x", "y",
	"start", "select",
	"#",".",
]

"""
# N64
VALID_INPUTS = [
	"left", "right", "up", "down",
	"dleft", "dright", "dup", "ddown",
	"cleft", "cright", "cup", "cdown",
	"a", "b", "l", "r", "z",
	"start",
	"#",".",
]
"""


"""
# Wii
VALID_INPUTS = [
	"left", "right", "up", "down",
	"pleft", "pright", "pup", "pdown",
	"tleft", "tright", "tup", "tdown",
	"a", "b", "one", "two", "minus", "plus",
	"c", "z",
	"shake", "point",
	"#", ".",
]
"""


# GC
"""
VALID_INPUTS = [
	"left", "right", "up", "down",
	"dleft", "dright", "dup", "ddown",
	"cleft", "cright", "cup", "cdown",
	"a", "b", "l", "r", "x", "y", "z",
	"start",
	"#",".",
]
"""

DURATION_DEFAULT = 200
DURATION_MAX = 60000

class Input:

	def __init__(self):

		self.name = ""
		self.hold = False
		self.release = False
		self.percent = 100
		self.duration = DURATION_DEFAULT
		self.duration_type = "ms"
		self.length = 0
		self.error = ""


INPUT_SYNONYMS = {
	#"pause": "start",
	"kappa": "#",
}


# Search and replace all known synonyms
def populate_synonyms(message):

	for name in INPUT_SYNONYMS:
		message = message.replace(name, INPUT_SYNONYMS[name])

	return message

# Returns Input object
def get_input(message):

	# Create a default input instance
	current_input = Input()

	# Check for input modifiers
	regex = r'[_-]'
	m = match(regex, message)

	# If theres a match, trim the message
	if m != None:
		c = message[m.start():m.end()]
		message = message[m.end():]

		if c == "_":
			current_input.hold = True
			current_input.length += 1
		elif c == "-":
			current_input.release = True
			current_input.length += 1

	# Try to match one input, prioritizing the longest match
	max = 0
	valid_input = ""

	for button in VALID_INPUTS:
		if button == ".":
			regex = r'' + "\." + r''
		else:
			regex = r'' + button + r''

		m = match(regex, message)

		if m != None:
			length = m.end() - m.start()

			if length > max:
				max = length
				current_input.name = message[m.start():m.end()]


	# If not a valid input, break parsing
	if current_input.name == "":
		current_input.error = "ERR_INVALID_INPUT"

		return current_input
	else:
		current_input.length += max

	# Trim the input from the message
	message = message[max:]

	# Try to match a percent
	regex = r'\d+%'
	m = match(regex, message)

	if m != None:
		current_input.percent = int(message[m.start():m.end()-1])
		message = message[m.end():]
		current_input.length += len(str(current_input.percent)) + 1

		if current_input.percent > 100:
			current_input.error = "ERR_INVALID_PERCENTAGE"
			return current_input

	# Try to match a duration
	regex = r'\d+'
	m = match(regex, message)

	if m != None:
		current_input.duration = int(message[m.start():m.end()])
		message = message[m.end():]
		current_input.length += len(str(current_input.duration))

		# Determine the type of duration
		regex = r'(s|ms)'
		m = match(regex, message)

		if m != None:
			current_input.duration_type = message[m.start():m.end()]
			message = message[m.end():]

			if current_input.duration_type == "s":
				current_input.duration *= 1000
				current_input.length += 1
			else:
				current_input.length += 2
		else:
			current_input.error = "ERR_DURATION_TYPE_UNSPECIFIED"
			return current_input

	if current_input.name == "start" and current_input.duration >= 500:
		current_input.error = "ERR_START_BUTTON_DURATION_MAX_EXCEEDED"
		return current_input

	return current_input

# Returns list containing: [Valid, input_sequence]
# Or: [Invalid, input that it failed on]
def parse(message):

	contains_start_input = False
	message = message.replace(" ", "").lower()
	input_subsequence = []
	input_sequence = []
	duration_counter = 0

	message = populate_synonyms(message)

	while len(message) > 0:

		input_subsequence = []
		subduration_max = 0
		current_input = get_input(message)

		"""
		if current_input.name == "plus":
			contains_start_input = True
		"""

		if current_input.error != "":
			return [False, current_input]

		message = message[current_input.length:]
		input_subsequence.append(current_input)

		if current_input.duration > subduration_max:
			subduration_max = current_input.duration

		if len(message) > 0:

			while message[0] == "+":

				if len(message) > 0:
					message = message[1:]
				else:
					break

				current_input = get_input(message)

				"""
				if current_input.name == "plus":
					contains_start_input = True
				"""

				if current_input.error != "":
					return [False, current_input]

				message = message[current_input.length:]
				input_subsequence.append(current_input)

				if current_input.duration > subduration_max:
					subduration_max = current_input.duration

				if len(message) == 0:
					break

		duration_counter += subduration_max

		if duration_counter > DURATION_MAX:
			current_input.error = "ERR_DURATION_MAX"
			return [False, current_input]

		input_sequence.append(input_subsequence)

	return [True, input_sequence, contains_start_input, duration_counter]
