#include <iostream>
#include <fstream>
#include <unordered_map>
#include <vector>
#include <string>
#include <iterator>
using namespace std;

vector<int> encode(string s1)
{
	cout << "Кодирование:\n";
	unordered_map<string, int> dictionary;
	for (int i = 0; i <= 255; i++) {
		string ch = "";
		ch += char(i);
		dictionary[ch] = i;
	}
	string p = "", c = "";
	p += s1[0];
	int code = 256;
	vector<int> output_code;
	for (int i = 0; i < s1.length(); i++) {
		if (i != s1.length() - 1)
			c += s1[i + 1];
		if (dictionary.find(p + c) != dictionary.end()) {
			p = p + c;
		}
		else {
			output_code.push_back(dictionary[p]);
			dictionary[p + c] = code;
			code++;
			p = c;
		}
		c = "";
	}
	output_code.push_back(dictionary[p]);
	return output_code;
}

string decode(vector<int> op)
{
	unordered_map<int, string> dictionary;
	for (int i = 0; i <= 255; i++) {
		string ch = "";
		ch += char(i);
		dictionary[i] = ch;
	}
	int old = op[0], n;
	string s = dictionary[old];
	string c = "";
	string output = "";
	c += s[0];
	output += s;
	int count = 256;
	for (int i = 0; i < op.size() - 1; i++) {
		n = op[i + 1];
		if (dictionary.find(n) == dictionary.end()) {
			s = dictionary[old];
			s = s + c;
		}
		else {
			s = dictionary[n];
		}
		output += s;
		c = "";
		c += s[0];
		dictionary[count] = dictionary[old] + c;
		count++;
		old = n;
	}
	return output;
}

bool auto_tester = true; //запуск автоматических тестов
void test_process(int test_amount, string path) { //test_amount - количество тестов, path - путь к папке с тестами
	string check = "", s = "";
	vector<int> decode_string;
	int n = 0;

	vector <string> tests_input(test_amount);
	vector <string> tests_output(test_amount);
	vector <string> tests_answer(test_amount);
	string file_name = "";

	fstream input_file;
	ofstream output_file;

	for (int i = 0; i < test_amount; i++) {
		tests_input[i] = file_name.append(path).append("\\").append(to_string(i + 1).append(".in"));
		file_name = "";
		tests_output[i] = file_name.append(path).append("\\").append(to_string(i + 1).append(".out"));
		file_name = "";
		tests_answer[i] = file_name.append(path).append("\\").append(to_string(i + 1).append(".ans"));
		file_name = "";
	}
	for (int j = 0; j < test_amount; j++) {
		input_file.open(tests_input[j]);
		output_file.open(tests_output[j]);
		if (!input_file) {
			output_file << "Файл входных данных не существует" << endl;
		}
		else if (input_file.peek() == EOF) {
			output_file << "Файл пуст" << endl;
		}
		else if (input_file) {
			getline(input_file, check);
			if (check == "encode") {
				decode_string.clear();
				if (input_file.peek() == EOF) {
					output_file << "Введен некорректный запрос!";
				}
				else {
					s.assign((istreambuf_iterator<char>(input_file.rdbuf())), istreambuf_iterator<char>());
					vector<int> output_code = encode(s);
					for (int i = 0; i < output_code.size(); i++) {
						if (i == output_code.size() - 1) {
							output_file << output_code[i];
						}
						else {
							output_file << output_code[i] << " ";
						}
					}
				}
			}
			else if (check == "decode") {
				decode_string.clear();
				copy(istream_iterator<int>(input_file), istream_iterator<int>(), inserter(decode_string, decode_string.begin()));
				if (decode_string.empty()) {
					output_file << "Введен некорректный запрос!";
				}
				else {
					if (decode_string[0] > 255) {
						output_file << "Введен некорректный запрос!";
						decode_string.clear();
					}
					else {
						while (!input_file.eof()) {
							input_file >> n;
							if (isdigit(n))
								decode_string.push_back(n);
							else {
								output_file << "Введен некорректный запрос!";
								decode_string.clear();
								break;
							}
						}
					}
					if (!decode_string.empty()) {
						string output_code = decode(decode_string);
						output_file << output_code;
					}
				}
			}
			else output_file << "Введен некорректный запрос!";
		}
		input_file.close();
		output_file.close();

		fstream output_file;
		fstream answer_file;

		output_file.open(tests_output[j]);
		answer_file.open(tests_answer[j]);
		string output_check = "";
		string answer_check = "";

		output_check.assign((istreambuf_iterator<char>(output_file.rdbuf())), istreambuf_iterator<char>());
		answer_check.assign((istreambuf_iterator<char>(answer_file.rdbuf())), istreambuf_iterator<char>());

		if (output_check == answer_check) {
			cout << j+1 << ")" << "OK\n";
		}
		else {
			cout << j+1 << ")" << "WA\n";
		}

	}
}

int main()
{
	if (auto_tester == false) {
		string check = "", s = "";
		vector<int> decode_string;
		int n = 0;
		fstream input_file;
		ofstream output_file;
		input_file.open("./LZWtest/input.txt");
		output_file.open("./output.txt");
		if (!input_file) {
			output_file << "Файл входных данных не существует" << endl;
		}
		else if (input_file.peek() == EOF) {
			output_file << "Файл пуст" << endl;
		}
		else if (input_file) {
			getline(input_file, check);
			if (check == "encode") {
				if (input_file.peek() == EOF) {
					output_file << "Введен некорректный запрос!";
				}
				else {
					s.assign((istreambuf_iterator<char>(input_file.rdbuf())), istreambuf_iterator<char>());
					vector<int> output_code = encode(s);
					for (int i = 0; i < output_code.size(); i++) {
						if (i == output_code.size() - 1) {
							output_file << output_code[i];
						}
						else {
							output_file << output_code[i] << " ";
						}
					}
				}
			}
			else if (check == "decode") {
				copy(istream_iterator<int>(input_file), istream_iterator<int>(), inserter(decode_string, decode_string.begin()));
				if (decode_string.empty()) {
					output_file << "Введен некорректный запрос!";
				}
				else {
					while (!input_file.eof()) {
						input_file >> n;
						if (isdigit(n))
							decode_string.push_back(n);
						else {
							output_file << "Введен некорректный запрос!";
							exit(0);
						}
					}
					if (decode_string[0] > 255) {
						output_file << "Введен некорректный запрос!";
						exit(0);
					}
					string output_code = decode(decode_string);
					output_file << output_code;
				}
			}
			else output_file << "Введен некорректный запрос!";
		}
		input_file.close();
		output_file.close();
		return 0;
	}
	else test_process(17, "C:\\LZWtest");
}
