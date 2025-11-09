#include <cstdio>
#include <unistd.h>
#include <cstring>
#include <vector>
#include <fcntl.h>
#include <string>

int main(int argc, char** argv)
{
    std::vector<char*> files;
    for(int i = 1; i < argc; i++)
    {
        files.push_back(argv[i]);
    }

    if(files.size() < 2)
    {
        char* text  = "Usage: \"mv {file1} {file2}\" or \"mv {file1} {file2} {directory}\"";
        write(1, text, strlen(text));
    }

    if(files.size() == 2)
    {
        int fd0 = open(files[0], O_RDONLY);
        int fd1 = open(files[1], O_WRONLY | O_CREAT, 0644);

        std::vector<char> buffer(1024);
        std::vector<char*> temp;
        size_t dump;

        while((dump = read(fd0, buffer.data(), buffer.size())) > 0)
        {
            temp.push_back(buffer.data());
        }
        close(fd0);

        for(size_t i = 0; i < temp.size(); i++)
        {
            write(fd1, temp[i], strlen(temp[i]));
        }
        close(fd1);

        remove(files[0]);
    }

    if(files.size() > 2)
    {
        std::vector<int> fd;

        for(size_t i = 0; i < files.size() - 1; i++)
        {
            fd.push_back(open(files[i], O_RDONLY));
        }

        for(size_t i = 0; i < fd.size(); i++)
        {
            std::vector<char> buffer(1024);
            std::vector<char*> temp;
            size_t dump;

            while((dump = read(fd[i], buffer.data(), buffer.size()) > 0))
            {
                temp.push_back(buffer.data());
            }

            close(fd[i]);

            std::string path = files[files.size() - 1] + std::string("/") + files[i];
            
            int copys = open(path.c_str(), O_CREAT | O_WRONLY, 0644);
            
            for(size_t i = 0; i < temp.size(); i++)
            {
                write(copys, temp[i], strlen(temp[i]));
            }

            close(copys);
            remove(files[i]);
        }



    }

}
