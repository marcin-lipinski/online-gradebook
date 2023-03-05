import json
import random

names = []
surnames = []
subjects_names = ["Polish", "Mathematics", "History", "English", "PE", "Computer science", "Physics", "Chemistry", "Geography", "Biology"]

school_classes = []
students = []
parents = []
subjects = []
teachers = []
accounts = []
school_classes_subjects_relations = []
grades = []


def load_names_and_surnames():
    with open("names.txt", "r", encoding='UTF-8') as file:
        for line in file:
            line = line[1:-3]
            names.append(line)

    with open("surnames.txt", "r", encoding='UTF-8') as file:
        for line in file:
            word = ""
            for char in line:
                if char != " ":
                    word += char
                else:
                    surnames.append(word)
                    break


def generate_classes():
    index = 1
    for letter in "abcd":
        for number in "12345678":
            dictionary = {
                "ID": "C" + str(index),
                "name": number + letter
            }
            school_classes.append(dictionary)
            index += 1
    create_json_file("school_classes", school_classes)


def generate_students():
    index = 1
    for school_class in school_classes:
        amount_of_students = random.randint(15, 25)
        for student in range(amount_of_students):
            name = names[random.randint(0, len(names) - 1)]
            surname = surnames[random.randint(0, len(surnames) - 1)]
            dictionary = {
                "ID": "S" + str(index),
                "name": name,
                "surname": surname,
                "schoolClass": school_class["ID"]
            }
            students.append(dictionary)
            index += 1
    create_json_file("students", students)


def generate_parents():
    index = 1
    for student in students:
        name = names[random.randint(0, len(names) - 1)]
        dictionary = {
            "ID": "P" + str(index),
            "name": name,
            "surname": student["surname"],
            "student": student["ID"]
        }
        parents.append(dictionary)
        index += 1
    create_json_file("parents", parents)


def generate_subjects():
    index = 1
    for subject in subjects_names:
        dictionary = {
            "ID": "SB" + str(index),
            "name": subject
        }
        subjects.append(dictionary)
        index += 1

    create_json_file("subjects", subjects)


def generate_teachers():
    index = 1
    amount_of_teachers = 90
    for teacher in range(amount_of_teachers):
        name = names[random.randint(0, len(names) - 1)]
        surname = surnames[random.randint(0, len(surnames) - 1)]
        dictionary = {
            "ID": "T" + str(index),
            "name": name,
            "surname": surname
        }
        teachers.append(dictionary)
        index += 1
    create_json_file("teachers", teachers)


def generate_accounts():
    for student in students:
        config = random_login_and_password("S")
        dictionary = {
            "login": config[0],
            "password": config[1],
            "subaccount": student["ID"]
        }
        accounts.append(dictionary)

    for parent in parents:
        config = random_login_and_password("P")
        dictionary = {
            "login": config[0],
            "password": config[1],
            "subaccount": parent["ID"]
        }
        accounts.append(dictionary)

    for teacher in teachers:
        config = random_login_and_password("T")
        dictionary = {
            "login": config[0],
            "password": config[1],
            "subaccount": teacher["ID"]
        }
        accounts.append(dictionary)

    create_json_file("accounts", accounts)


def generate_grades():
    index = 1
    for student in students:
        for subject in subjects:
            for i in range(random.randint(1, 10)):
                dictionary = {
                    "ID": "G" + str(index),
                    "student": student["ID"],
                    "subject": subject["ID"],
                    "value": random.randint(1, 6)
                }
                grades.append(dictionary)
                index += 1
    create_json_file("grades", grades)


def generate_school_classes_subjects_relations():
    for school_class in school_classes:
        for subject in subjects:
            dictionary = {
                "schoolClass": school_class["ID"],
                "subject": subject["ID"],
                "teacher": teachers[random.randint(0, len(teachers) - 1)]["ID"]
            }
            school_classes_subjects_relations.append(dictionary)
    create_json_file("school_classes_subjects_relations", school_classes_subjects_relations)


def create_json_file(filename, source):
    json_object = json.dumps(source, indent=4)
    with open(filename + ".json", "w") as outfile:
        outfile.write(json_object)


def random_login_and_password(account_type_letter):
    letters = "abcdefghijklmnoprstuwxyz"
    numbers = "1234567890"
    login = account_type_letter
    password = ""

    while login in [account["login"] for account in accounts] or login == account_type_letter:
        login = account_type_letter
        for i in range(6):
            login += numbers[random.randint(0, len(numbers) - 1)]

    for i in range(10):
        password += (letters + numbers)[random.randint(0, len((letters + numbers)) - 1)]

    return login, password


load_names_and_surnames()
generate_classes()
generate_students()
generate_parents()
generate_teachers()
generate_subjects()
generate_accounts()
generate_grades()
generate_school_classes_subjects_relations()
