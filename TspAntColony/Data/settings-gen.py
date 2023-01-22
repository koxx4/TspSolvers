import os
import re

def extract_number_from_file_name(file_name):
    match = re.search(r'\d+', file_name)
    if match:
        return int(match.group(0))
    return 0

header_comment = "#nazwa instancji,ilość powtórzeń,optymalny koszt,optymalna ścieżka,rozkład feromonu,alfa,beta\n"

folder_path = './'
instances_names = os.listdir(folder_path)

instances_names.remove("settings.csv")
instances_names.remove("settings-gen.py")
instances_names.remove("settings-test.csv")

instances_names = sorted(instances_names, key=extract_number_from_file_name)

print( f"Found instances: {instances_names}")

algo_repetitions = 30
pheromones_spread = ["DAS", "QAS"]
alpha_values = ["1", "2", "3"]
beta_values = ["1", "2", "3"]

with open('settings-test.csv', 'w') as f:
    f.write(header_comment)

    for instance in instances_names:
        for ps in pheromones_spread:
            for alpha in alpha_values:
                for beta in beta_values:
                    f.write(f"{instance},{algo_repetitions},A-{instance}-A,[0],{ps},{alpha},{beta}\n")