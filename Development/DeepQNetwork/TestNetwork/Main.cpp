#include <iostream>
#include <vector>
#include <math.h>
#include "NeuralNetwork.h"

double loss(std::vector<double> out, std::vector<double> expected)
{
	double sum = 0;
	for (int i = 0; i < 3; ++i)
	{
		sum += (out[i] - expected[i]) * (out[i] - expected[i]);
	}
	return sum / 3;
}

double sigmoid(double x)
{
	return 1.0 / (1 + exp(-x));
}

std::vector<double> feedForward(std::vector<double> x, double w1[30][5], double w2[5][3])
{
	double sum1[5] = { 0 };
	for (int j = 0; j < 5; ++j)
	{
		for (int i = 0; i < 30; ++i)
		{
			sum1[j] += x[i] * w1[i][j];
		}
	}

	double a1[5] = { 0 };
	for (int j = 0; j < 5; ++j)
	{
		a1[j] = sigmoid(sum1[j]);
	}

	// second layer
	double sum2[3] = { 0 };
	for (int j = 0; j < 3; ++j)
	{
		for (int i = 0; i < 5; ++i)
		{
			sum2[j] += a1[i] * w2[i][j];
		}
	}

	std::vector<double> a2;
	for (int j = 0; j < 3; ++j)
	{
		a2.push_back(sigmoid(sum2[j]));
		std::cout << a2[j] << std::endl;
	}

	return a2;
}

int main()
{
	NeuralNetwork *nn = new NeuralNetwork(0.01);
	nn->addLayer(3, 3, "RELU");
	nn->addLayer(2, 3, "LINEAR");

	NeuralNetwork* n1 = new NeuralNetwork(*nn);
	NeuralNetwork* n2 = new NeuralNetwork(*nn);

	n1->feedForward({1, 0, 1});
	n1->backPropagate({ 1, 1, 0 });

	n2->copyWights(*n1);

	n1->save("s1");
	n2->save("s2");

	std::cout << n1->outputSize() << std::endl;

	/*nn->addLayer(12, 24, "RELU");
	nn->addLayer(4, 12, "LINEAR");
	nn->save("neural_network.json");
	nn->load("neural_network.json");*/
	//nn->addLayer(5, 30, "SIGMOID");
	//nn->addLayer(3, 5, "SIGMOID");
	////nn.load();
	//NeuralNetwork* nn2 = new NeuralNetwork(0);
	//*nn2 = *nn;

	///*double w1[30][5] = { {-0.10949782, -0.04781567, -0.25968173, 0.01499938, -2.09183501},
	//{1.40350242, -1.44757798, -1.24967533, -0.30988883, -0.30067187},
	//{1.3868396, 0.95110953, 1.14152294, -1.84284848, 0.33570576},
	//{0.0938922, 0.12402201, 0.47700265, 0.75316117, 0.44318564},
	//{-0.31911738, -0.83083504, -1.35965268, 1.29369707, 0.48847561},
	//{0.42484033, 0.61925595, -1.02295048, -1.00711151, 0.03914471},
	//{-0.22581572, 2.1311519, 0.12812969, 1.39229903, -0.3791329},
	//{-0.44932479, -0.19773947, -1.21510655, -0.39541597, 0.62557401},
	//{1.84455723, 0.147465, 1.21123821, -0.72401633, -1.73905178},
	//{-0.70269367, 0.77602151, 0.49308295, -0.80719825, -0.19466084},
	//{1.25878576, 0.33547694, 0.47999575, -0.04200924, -1.53708535},
	//{0.03544121, 0.52573406, -0.20464281, 0.46213737, 3.57027577},
	//{0.01739651, 0.16235976, -1.31885953, 1.23317935, 0.16778835},
	//{0.33631064, -0.69991143, -0.87786197, 2.36095297, 1.69348633},
	//{0.30851708, -0.80496301, -0.42512636, -0.57365818, -1.00805777},
	//{0.56075004, -2.4811168, -1.9055633, -0.04064191, 0.38568668},
	//{0.61775337, -0.92727726, 1.15310294, -1.24832159, -1.6824513},
	//{-0.7490321, 2.12538307, -0.98690508, -0.40114543, -1.87565378},
	//{0.58705772, 0.74501867, -0.58962515, -0.10108693,  1.80467275},
	//{0.98106838, 0.6694859,   0.24982339,  0.75756562, -0.86256313},
	//{-0.10180785, 0.21945717, -0.61450864, -1.21265616, -0.99341594},
	//{-1.0208929, -0.85886242, -1.41500571, -0.17446187,  0.718512},
	//{1.41476287, -0.46596645, -0.28535693, -1.28938881,  0.6019126},
	//{-0.38606421, -1.06024216, -0.09858542,  1.82572292, -0.33187678},
	//{1.40254826, 0.50731202, -0.6574863,   0.87276015, -0.08533494},
	//{0.07617277, 0.29362206, -0.52360477,  0.05362727, -1.06846622},
	//{1.42498312, -0.5211035,   0.35489985, -1.03433563,  0.68438897},
	//{0.78432096, -1.8915836, -2.04438108, -1.10662088, -0.03862058},
	//{-0.85159717, -0.1771978, -0.52175963,  0.19893721, -0.3770596},
	//{0.86845296, 0.73780719, -1.7725819, -1.01265084, -0.3201537},
	//};

	//for (int j = 0; j < 5; ++j)
	//{
	//	for (int i = 0; i < 30; ++i)
	//	{
	//		nn.layers[0][j]->weights[i] = w1[i][j];
	//	}
	//}

	//double w2[5][3] = { {-0.98607697, - 1.40716412, - 0.70890308},
	//{-0.9252172, - 1.74661961, - 1.23016681 },
	//{0.94362536,  1.48926619, - 1.68763275  },
	//{1.28343098,  0.55655674,  1.06759053},
	//{0.9204753,   0.87474623, - 0.63372866} };

	//for (int j = 0; j < 3; ++j)
	//{
	//	for (int i = 0; i < 5; ++i)
	//	{
	//		nn.layers[1][j]->weights[i] = w2[i][j];
	//	}
	//}*/

	//std::vector<double> x1 = { 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1 };
	//std::vector<double> x2 = { 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0 };
	//std::vector<double> x3 = { 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0 };

	//std::vector<double> y1 = { 1, 0, 0 };
	//std::vector<double> y2 = { 0, 1, 0 };
	//std::vector<double> y3 = { 0, 0, 1 };

	//for (int epoch = 0; epoch < 2000; ++epoch)
	//{
	//	double sum = 0;
	//	auto o1 = nn->feedForward(x1);
	//	nn->backPropagate(y1);
	//	sum += pow((y1[0] - o1[0]), 2) + pow((y1[1] - o1[1]), 2) + pow((y1[2] - o1[2]), 2);
	//	/*for (int i = 0; i < 3; ++i)
	//	{
	//		std::cout << o1[i] << " ";
	//	}
	//	std::cout << std::endl;*/

	//	auto o2 = nn->feedForward(x2);
	//	nn->backPropagate(y2);
	//	sum += pow((y2[0] - o2[0]), 2) + pow((y2[1] - o2[1]), 2) + pow((y2[2] - o2[2]), 2);
	//	/*for (int i = 0; i < 3; ++i)
	//	{
	//		std::cout << o2[i] << " ";
	//	}
	//	std::cout << std::endl;*/

	//	auto o3 = nn->feedForward(x3);
	//	nn->backPropagate(y3);
	//	sum += pow((y3[0] - o3[0]), 2) + pow((y3[1] - o3[1]), 2) + pow((y3[2] - o3[2]), 2);
	//	/*for (int i = 0; i < 3; ++i)
	//	{
	//		std::cout << o3[i] << " ";
	//	}
	//	std::cout << std::endl;*/

	//	//std::cout << "loss_sum = " << sum << std::endl;
	//	std::cout << "epoch " << epoch + 1 << " ===== acc: " << 1 - sum << " ======= loss: " << sum << " +++++++++++ learning_rate: " << std::endl;
	//}

	//std::vector<double> x4 = { 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1 };
	//auto out = nn->feedForward(x4);
	////std::cout << *nn.layers[1]->neurons[0]->output << " " << *nn.layers[1]->neurons[1]->output << " " << *nn.layers[1]->neurons[2]->output << std::endl;

	//auto max = out[0];
	//int idx = 0;
	//for (int i = 1; i < 3; ++i)
	//{
	//	if (max < out[i])
	//	{
	//		max = out[i];
	//		idx = i;
	//	}
	//}

	//switch (idx)
	//{
	//case 0:
	//	std::cout << "A";
	//	break;
	//case 1:
	//	std::cout << "B";
	//	break;
	//case 2:
	//	std::cout << "C";
	//	break;
	//}

	////std::cout << std::endl;

	////nn->save();

	////	NeuralNetwork nn(0.001);
	////	nn.addLayer(2, 2, "SIGMOID");
	////	nn.addLayer(2, 2, "SIGMOID");
	////
	////	nn.layers[0][0]->weights[0] = 0.13436424411240122;
	////	nn.layers[0][0]->weights[1] = 0.8474337369372327;
	////	nn.layers[0][0]->biasWeight = 0.763774618976614;
	////	nn.layers[0][1]->weights[0] = 0.2550690257394217;
	////	nn.layers[0][1]->weights[1] = 0.49543508709194095;
	////	nn.layers[0][1]->biasWeight = 0.4494910647887381;
	////
	////	nn.layers[1][0]->weights[0] = 0.651592972722763;
	////	nn.layers[1][0]->weights[1] = 0.7887233511355132;
	////	nn.layers[1][0]->biasWeight = 0.0938595867742349;
	////	nn.layers[1][1]->weights[0] = 0.02834747652200631;
	////	nn.layers[1][1]->weights[1] = 0.8357651039198697;
	////	nn.layers[1][1]->biasWeight = 0.43276706790505337;
	////
	////	std::vector<std::vector<double>> dataset = { {2.7810836, 2.550537003},
	////		{1.465489372, 2.362125076},
	////		{3.396561688, 4.400293529},
	////		{1.38807019, 1.850220317},
	////		{3.06407232, 3.005305973},
	////		{7.627531214, 2.759262235},
	////		{5.332441248, 2.088626775},
	////		{6.922596716, 1.77106367},
	////		{8.675418651, -0.242068655},
	////		{7.673756466, 3.508563011}};
	////
	////	std::vector<std::vector<double>> expected = { {1, 0},
	////{1, 0},
	////{1, 0},
	////{1, 0},
	////{1, 0},
	////{0, 1},
	////{0, 1},
	////{0, 1},
	////{0, 1},
	////{0, 1} };
	////
	////	for (int e = 0; e < 500; ++e)
	////	{
	////		double sum = 0;
	////		for (int d = 0; d<dataset.size(); ++d)
	////		{
	////			auto out = nn.feedForward(dataset[d]);
	////			/*std::cout << expected[d][0] << " " << out[0] << std::endl;
	////			std::cout << expected[d][1] << " " << out[1] << std::endl;
	////			std::cout << pow((expected[d][0] - out[0]), 2) + pow((expected[d][1] - out[1]), 2) << std::endl;*/
	////			sum += pow((expected[d][0] - out[0]), 2) + pow((expected[d][1] - out[1]), 2);
	////
	////			nn.backPropagate(expected[d]);
	////		}
	////
	////		std::cout << "epoch=" << e + 1 << " lRate=" << 0.5 << " error=" << sum << std::endl;
	////	}

	//	/*std::vector<double> x = {1, 0};
	//	auto out = nn.feedForward(x);
	//	std::cout << out[0] << " " << out[1] << std::endl;
	//	nn.backPropagate(out, { 0, 1 });*/
}