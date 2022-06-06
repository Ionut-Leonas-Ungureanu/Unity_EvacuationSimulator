using Assets.Scripts.Prefabs.Bot.States.Constants;
using System;
using UnityEngine;

namespace Assets.Scripts.Utils.Experience
{
    class ExperienceContainer
    {
        private readonly double[] _initial;
        private readonly double[] _final;
        private readonly int _action;
        private readonly double _reward;
        private readonly bool _isOver;

        public double[] Initial => _initial;
        public double[] Final => _final;
        public int Action => _action;
        public bool IsOver => _isOver;
        public double Reward => _reward;

        public ExperienceContainer(double[] initial, int action, double[] final, double reward, bool isOver)
        {
            _initial = initial;
            _final = final;
            _action = action;
            _reward = reward;
            _isOver = IsOver;
        }
    }
}
