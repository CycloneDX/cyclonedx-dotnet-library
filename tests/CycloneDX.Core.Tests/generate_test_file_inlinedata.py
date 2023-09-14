# run this from the test resource file directory, i.e. Resources/v1.5

import os
import os.path

xml = []
json = []
protobuf = []

dir_entries = os.listdir(".")
dir_entries.sort()
for entry_name in dir_entries:
    if os.path.isfile(entry_name):
        inline_data = f"[InlineData(\"{entry_name}\")]"
        if entry_name.endswith(".xml"):
            xml.append(inline_data)
        elif entry_name.endswith(".json"):
            json.append(inline_data)
        elif entry_name.endswith(".textproto"):
            protobuf.append(inline_data)

print("\nXML Entries\n")
print("\n".join(xml))
print("\nJSON Entries\n")
print("\n".join(json))
print("\nProtobuf Entries\n")
print("\n".join(protobuf))
