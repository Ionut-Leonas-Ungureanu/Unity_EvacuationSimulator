#pragma once
#include <vector>

class ExperienceContainer
{
public:
	std::vector<double> initialState;
	int action;
	std::vector<double> reachedState;
	double reward;
	bool gameOver;
};