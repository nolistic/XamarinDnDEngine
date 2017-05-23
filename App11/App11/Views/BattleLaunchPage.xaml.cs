﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App11.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using SQLite;

namespace App11.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BattleLaunchPage : ContentPage
	{
        public Results battleResults;
        Queue<Fighter> charQueue = new Queue<Fighter>();
        ScoreBoard gameScore = new ScoreBoard();

		public string deadTeamInfo = "";
        

        public BattleLaunchPage()
		{
			InitializeComponent();

			initializeCharQ();

            //for(int i =0; i<4; i++)
            //{
            //    charQueue.Enqueue(new Character(2, 2, 2, i + 1, 10, 0, "Character " + (i+1)));
            //}
		}

		/*protected override async void OnAppearing()
        {
            base.OnAppearing();
            battleStart();
            
        }
        */

		private async Task initializeCharQ()
		{
			List<Character> charItems = await App.Database.GetCharactersAsync();

			foreach (Character charItem in charItems)
			{
				//public Character(int strength, int defense, int speed, int stackOrder, int hitPoints,int level)
				int str = charItem.Strength;
				int def = charItem.Defense;
				int spd = charItem.Speed;
				int stkod = charItem.StackOrder;
				int hp = charItem.HitPoints;
				int lv = charItem.Level;
				string name = charItem.Name;

				//charQueue.Enqueue(new Character(2, 2, 2, charItem.ID, 10, 0, "Character " + charItem.ID));
				charQueue.Enqueue(new Character(str, def, spd, stkod, hp, lv, name));
			}
		}
        //this function is if mike presses it and he gets to the end fast.
        public async void MikeStart(object sender, EventArgs e)
        {
            gameScore.deadChars = new ObservableCollection<Character>();
            while (charQueue.Count != 0)
            {
                gameScore.round += 1;
                BattleController newBattle = new BattleController(charQueue);
                battleResults = newBattle.initBattle();
               
                if (battleResults.deadChars.Count != 0)
                {
                    foreach (Character deadChar in battleResults.deadChars)
                    {
						// add charInfo to deadTeamInfo
						deadTeamInfo = deadTeamInfo + deadChar.getCharInfo() + "\n\n";
                        gameScore.deadChars.Add(deadChar);
                    }
					// set the teamInfo and clear deadTeamInfo for the next game
					gameScore.teamInfo += deadTeamInfo;
					deadTeamInfo = "";
                }

                if (charQueue.Count != 0)
                {
                    Random itemDist = new Random();
                    int charAwarded = itemDist.Next(0, charQueue.Count);
                    gameScore.currScore += battleResults.points;
                    for (int i = 0; i < charQueue.Count; i++)
                    {
                        Character currChar = (Character)charQueue.Dequeue();
                        charQueue.Enqueue(currChar);
                        if (currChar.AwardExp((int)battleResults.points / 4))
                        {
                            battleResults.postGame.Add(currChar.Name + " leveled up to level " + currChar.Level);
                        }
                        if (i == charAwarded)
                        {
                            battleResults.postGame.Add(currChar.Name + " looted a " + battleResults.loot.Name + " which increases "
                                + battleResults.loot.Attribute + " by " + battleResults.loot.Strength);
                            if (battleResults.loot.Attribute == "Strength")
                            {
                                currChar.Strength += battleResults.loot.Strength;
                                currChar.strItem = battleResults.loot;
                            }
                            else if (battleResults.loot.Attribute == "Defense")
                            {
                                currChar.Defense += battleResults.loot.Strength;
                                currChar.defItem = battleResults.loot;
                            }
                            else if (battleResults.loot.Attribute == "Speed")
                            {
                                currChar.Speed += battleResults.loot.Strength;
                                currChar.speedItem = battleResults.loot;
                            }
                            else if (battleResults.loot.Attribute == "HP")
                            {
                                battleResults.postGame.Add(currChar.Name + " looted a " + battleResults.loot.Name + " which heals them by "
                                    + battleResults.loot.Strength);
                                //healing items will be implemented here.
                                if (currChar.HitPoints == currChar.maxHP)
                                {
                                    battleResults.postGame.Add(currChar.Name + " is already full health!");
                                }
                                else if (currChar.HitPoints + battleResults.loot.Strength >= currChar.maxHP)
                                {
                                    currChar.HitPoints = currChar.maxHP;
                                    battleResults.postGame.Add(currChar.Name + " is healed to full health!");
                                }
                                else
                                {
                                    currChar.HitPoints += battleResults.loot.Strength;
                                    battleResults.postGame.Add(currChar.Name + " is healed to " + currChar.HitPoints + "!");
                                }
                            }
                            //currChar.AddItemToInv(battleResults.loot);
                        }
                    }
                }
            }
            await Navigation.PushAsync(new GameOver(gameScore));

        }
		public async void battleStart(object sender, EventArgs e)
        {
            if (charQueue.Count == 0)
            {
                await Navigation.PushAsync(new GameOver(gameScore));
            }
            else
            {
                gameScore.round += 1;
                BattleController newBattle = new BattleController(charQueue);
                battleResults = newBattle.initBattle();
                gameScore.deadChars = new ObservableCollection<Character>();
                if (battleResults.deadChars.Count != 0)
                {

                    foreach (Character deadChar in battleResults.deadChars)
                    {
						// add charInfo to deadTeamInfo
						deadTeamInfo = deadTeamInfo + deadChar.getCharInfo() + "\n\n";
						gameScore.deadChars.Add(deadChar);
                    }
					// set the teamInfo and clear deadTeamInfo for the next game
					gameScore.teamInfo += deadTeamInfo;
					deadTeamInfo = "";

				}
                
                if (charQueue.Count != 0)
                {
                    Random itemDist = new Random();
                    int charAwarded = itemDist.Next(0, charQueue.Count);
                    gameScore.currScore += battleResults.points;
                    for (int i = 0; i < charQueue.Count; i++)
                    {
                        Character currChar = (Character)charQueue.Dequeue();
                        charQueue.Enqueue(currChar);
                        if (currChar.AwardExp((int)battleResults.points / 4))
                        {
                            battleResults.postGame.Add(currChar.Name + " leveled up to level " + currChar.Level);
                        }
                        if (i == charAwarded)
                        {
                            battleResults.postGame.Add(currChar.Name + " looted a " + battleResults.loot.Name + " which increases "
                                + battleResults.loot.Attribute + " by " + battleResults.loot.Strength);
                            if (battleResults.loot.Attribute == "Strength")
                            {
                                currChar.Strength += battleResults.loot.Strength;
                                currChar.strItem = battleResults.loot;
                            }
                            else if (battleResults.loot.Attribute == "Defense")
                            {
                                currChar.Defense += battleResults.loot.Strength;
                                currChar.defItem = battleResults.loot;
                            }
                            else if (battleResults.loot.Attribute == "Speed")
                            {
                                currChar.Speed += battleResults.loot.Strength;
                                currChar.speedItem = battleResults.loot;
                            }
                            else
                            {
                                currChar.HitPoints += battleResults.loot.Strength;
                                currChar.hpItem = battleResults.loot;
                            }

                        }
                    }
                }

                await Navigation.PushAsync(new BattleDetailPage(battleResults, charQueue, gameScore));
            }
            
           
        }
	}
}
