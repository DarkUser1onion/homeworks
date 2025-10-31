CXX = g++
CXXFLAGS = -Wall -O3 -march=native

MODULES_DIR = modules
MAIN_SRC = main.cpp
TARGET = main

MODULES_SRC = $(wildcard $(MODULES_DIR)/*.cpp)
MODULES_SO = $(patsubst $(MODULES_DIR)/%.cpp,$(MODULES_DIR)/%.so,$(MODULES_SRC))

all: $(TARGET) $(MODULES_SO)


$(TARGET): $(MAIN_SRC)
	$(CXX) $(CXXFLAGS) -o $@ $< -ldl

$(MODULES_DIR)/%.so: $(MODULES_DIR)/%.cpp
	$(CXX) $(CXXFLAGS) -fPIC -shared -o $@ $<

clean:
	rm -f $(TARGET) $(MODULES_DIR)/*.so

.PHONE: all clean
