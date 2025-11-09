#include <cstring>
#include <string>
#include <unistd.h>
#include <fcntl.h>
#include <string.h>
#include <vector>

int main(int argc, char** argv)
{
    std::vector<char*> files;

    for(size_t i = 1; i < argc; i++)
    {
        files.push_back(argv[i]);
    }

    if(files.size() < 2)
    {
        char* text  = "Usage: \"cp {file1} {file2}\" or \"cp {file1} {file2} {directory}\"";
        write(1, text, strlen(text));
    }

    
    if(files.size() == 2)
    {
        int file0 = open(files[0], O_RDONLY);
        int file1 = open(files[1], O_CREAT | O_WRONLY | O_APPEND, 0644);
    
        std::vector<char> buffer(1024);
        std::vector<char*> temp;
        size_t dump;

        while((dump = read(file0,buffer.data(),buffer.size()) > 0))
        {
            temp.push_back(buffer.data());
        }

        for(size_t i = 0; i < temp.size(); i++)
        {
            write(file1, temp[i], strlen(temp[i]));
        }

        close(file0);
        close(file1);

        return 0;
    }

    if(files.size() > 2)
    {
        std::vector<int> fd;

        for(size_t i = 0; i < files.size() - 1; i++)
        {
            fd.push_back(open(files[i], O_RDONLY));
        }

        for(int i = 0; i < fd.size(); i++)
        {
            size_t dump;
            std::vector<char*> temp;
            std::vector<char> buffer(1024);
            
            while((dump = read(fd[i], buffer.data(), buffer.size()) > 0))
            {
                temp.push_back(buffer.data());
            }

            std::string path = std::string(files[files.size() - 1]) + "/" + files[i];

            int copys = open(path.c_str(), O_WRONLY | O_CREAT, 0644);

            for(size_t i = 0; i < temp.size(); i++)
            {
                write(copys, temp[i], strlen(temp[i]));
            }

            close(fd[i]);
            close(copys);
        }

    }



}
