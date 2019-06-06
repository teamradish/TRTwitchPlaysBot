import re

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

VALID_INPUTS = [
	"left", "right", "up", "down",
	"a", "b", "l", "r", "x", "y",
	"start", "select",
	"#",".",
]


def expandify(message):
	#m = re.search("(\[[^\[\]]*)\]\*(\d{1,2})", message)
	m = re.search("\[" + "([^\[\]]*\])" + "\*" + "(\d{1,2})" , message)
	while m:
		new = ""
		#value = m.group(1)[1:]
		value = m.group(1).replace("]","").replace("[","")
		try: number = int(m.group(2))
		except: return message
		for i in range(0,number): new += value
		message = message[0:m.start()] + new + message[m.end():]
		m = re.search("\[" + "([^\[\]]*\])" + "\*" + "(\d{1,2})" , message)
	return message

def populate_variables(macro_contents, variables):
	i = 0
	for v in variables:
		macro_contents = re.sub("<" + str(i) + ">", v, macro_contents)
		i += 1
	return macro_contents

def populate_macros(message):
	global MACROS

	message = message.replace(" ","")
	message = expandify(message)

	MAX_RECURSION = 10
	count = 0
	found_macro = True
	while count < MAX_RECURSION and found_macro:
		found_macro = False
		possible_macros = re.finditer("#[a-zA-Z0-9\(\,\.]*", message)
		subs = []
		for p in possible_macros:
			macro_name = re.sub("\(.*\)","",message[p.start():p.end()])
			macro_name_generic = ""
			arg_index = macro_name.find("(")
			if arg_index != -1:
				macro_args = re.search("\(.*\)",message[p.start():p.end()+1])
				if macro_args:
					macro_args = message[p.start()+macro_args.start()+1:p.end()].split(",")
					macro_name += ")"
					macro_name_generic = re.sub("\(.*\)","", macro_name) + "("
					for i in range(len(macro_args)): macro_name_generic += "*,"
					macro_name_generic = macro_name_generic[:-1] + ")"
			else:
				macro_args = []
				macro_name_generic = macro_name

			longest = ""
			for macro in TPE["MACROS"]:
				if macro in macro_name_generic:
					if len(macro) > len(longest): longest = macro
				end = p.start() + len(longest)
			if longest != "":
				if len(macro_args) > 0: subs.append((longest, (p.start(),p.end()+1), macro_args))
				else: subs.append((longest, (p.start(),end), macro_args))

		subs = sorted(subs, key=lambda x: x[1][0])
		new = ""
		if len(subs) > 0:
			found_macro = True
			new = message[0:subs[0][1][0]]
			prev = None
			for current in subs:
				if prev != None: new += message[prev[1][1]:current[1][0]]
				new += populate_variables(TPE["MACROS"][current[0]], current[2])
				prev = current
			new += message[current[1][1]:]
			message = new
		count += 1
	return message

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
	m = re.match(regex, message)

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

		m = re.match(regex, message)

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
	m = re.match(regex, message)

	if m != None:
		current_input.percent = int(message[m.start():m.end()-1])
		message = message[m.end():]
		current_input.length += len(str(current_input.percent)) + 1

		if current_input.percent > 100:
			current_input.error = "ERR_INVALID_PERCENTAGE"
			return current_input

	# Try to match a duration
	regex = r'\d+'
	m = re.match(regex, message)

	if m != None:
		current_input.duration = int(message[m.start():m.end()])
		message = message[m.end():]
		current_input.length += len(str(current_input.duration))

		# Determine the type of duration
		regex = r'(s|ms)'
		m = re.match(regex, message)

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

# USAGE:
message = "[a left right]*3"
parse_message = expandify(populate_macros(message))
parse_result = parse(parse_message)
print(parse_result)
