#include <vector>

using namespace std;

extern "C" void process(const vector<int>& input, vector<int>& out)
{
	for(int num: input)
	{
		out.push_back(num / 5);
	}
}
