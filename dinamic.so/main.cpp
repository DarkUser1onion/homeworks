#include <iostream>
#include <vector>
#include <dlfcn.h>
#include <filesystem>

using namespace std;

using ProcessFunc = void (*)(const vector<int>&, vector<int>&);


int main()
{
	vector<int> numbers;
	int num;
	size_t sizeNum;

	cout << "Скока чисел вводим?: "; cin >> sizeNum;

	cout << "Введите последователь чисел: ";

	for(size_t i = 0; i < sizeNum; i++)
	{
		cin >> num;
		numbers.push_back(num);
	}


	vector<string> modules;
	for(const auto& modul : filesystem::directory_iterator("./modules"))
		if(modul.path().extension() == ".so")
			modules.push_back(modul.path());



	cout << "Все модули в директории \"modules\"" << endl;
	for(size_t i = 0; i < modules.size(); i++)
		cout << i+1 << " модуль: " << modules[i] << endl;
	int choiceModule;

	cout << "Ентер нумбер модуля: ";
	cin >> choiceModule;

	void* handle = dlopen(modules[choiceModule - 1].c_str(), RTLD_LAZY);

	if(!handle)
	{
		cerr << "Ошибка загрузки модуля!!" << endl << dlerror();
		dlclose(handle);
		return 404;
	}

	auto process = reinterpret_cast<ProcessFunc>(dlsym(handle, "process"));

	if(!process)
	{
		cerr << "Ошибка загрузки модуля!!" << endl << dlerror();
		dlclose(handle);
		return 404;
	}

	vector<int> result;

	process(numbers, result);

	for(auto value: result)
		cout << value << " ";

	cout << endl;

	dlclose(handle);

	return 0;
}
