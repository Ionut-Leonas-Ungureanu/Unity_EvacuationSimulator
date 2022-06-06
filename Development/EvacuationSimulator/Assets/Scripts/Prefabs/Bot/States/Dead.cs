﻿using Assets.Scripts.Prefabs.Bot.States.Context;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Bot.States
{
    class Dead : FinishState
    {
        public Dead(BotContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            _context.Status = BotStatus.DEAD;
            base.HandleState();

            _context.Bot.Dispatcher.Schedule(() =>
            {
                _context.Bot.Hide();
            }).WaitOne();

            Debug.Log("Bot Died!");
        }
    }
}
