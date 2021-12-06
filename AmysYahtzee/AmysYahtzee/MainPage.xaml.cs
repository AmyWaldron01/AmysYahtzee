using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AmysYahtzee
{
    public partial class MainPage : ContentPage
    {
        const int MAX_ROLLS = 3;
        int _rollNumber = 1;
        Random _random;

        //Setting varibles for bottom Score
        int Set3;
        int Set4;
        int SetFull;
        int SetSmall;
        int SetLong;
        int SetYahtzee;
        int SetChance;
        int SetYahBonus;

        int ones, twos, threes, fours, fives, sixs;

        bool[] KeepScore;

        bool stopYah;

        public MainPage()
        {
            InitializeComponent();
            NewGame();


        }

        private void BtnDiceRoll_Clicked(object sender, EventArgs e)
        {
            int r1, r2, r3, r4, r5;

            // provide feedback as to whyat Roll they are on
            LblFeedback.Text = "Roll Number " + _rollNumber.ToString();

            // generate 5 random numbers
            if (_random == null) _random = new Random();
            r1 = _random.Next(1, 7);
            r2 = _random.Next(1, 7);
            r3 = _random.Next(1, 7);
            r4 = _random.Next(1, 7);
            r5 = _random.Next(1, 7);

            // assign them to the dice
            if (BtnDice1.StyleId == "Roll") BtnDice1.Text = r1.ToString();
            if (BtnDice2.StyleId == "Roll") BtnDice2.Text = r2.ToString();
            if (BtnDice3.StyleId == "Roll") BtnDice3.Text = r3.ToString();
            if (BtnDice4.StyleId == "Roll") BtnDice4.Text = r4.ToString();
            if (BtnDice5.StyleId == "Roll") BtnDice5.Text = r5.ToString();

            // update the turn number
            //Stops user from rolling after 3
            if (_rollNumber >= MAX_ROLLS)
            {
                BtnDiceRoll.IsEnabled = false;
                LblFeedback.Text = "Choose a score...";
                CalculatePredictedScores();
            }
            _rollNumber++;


            //Renable all the buttons
            if (!KeepScore[0])
            {
                BtnOnes.IsEnabled = true;
            }

            if (!KeepScore[1])
            {
                BtnTwos.IsEnabled = true;
            }

            if (!KeepScore[2])
            {

                BtnThrees.IsEnabled = true;
            }

            if (!KeepScore[3])
            {
                BtnFours.IsEnabled = true;
            }

            if (!KeepScore[4])
            {
                BtnFives.IsEnabled = true;
            }

            if (!KeepScore[5])
            {
                BtnSixes.IsEnabled = true;
            }


            if (!KeepScore[6])
            {
                threeOfAKindBtn.IsEnabled = true;
            }

            if (!KeepScore[7])
            {
                fourOfAKindBtn.IsEnabled = true;
            }

            if (!KeepScore[8])
            {
                fullHouseBtn.IsEnabled = true;
            }

            if (!KeepScore[9])
            {
                smallBtn.IsEnabled = true;
            }

            if (!KeepScore[10])
            {
                longBtn.IsEnabled = true;
            }

            if (!KeepScore[11])
            {
                YahtzeeBtn.IsEnabled = true;
            }

            if (!KeepScore[12])
            {
                ChanceBtn.IsEnabled = true;
            }


            //Renabale ALL Dice
            BtnDice1.IsEnabled = true;
            BtnDice2.IsEnabled = true;
            BtnDice3.IsEnabled = true;
            BtnDice4.IsEnabled = true;
            BtnDice5.IsEnabled = true;

        }

        private void BtnSetRollStatus_Clicked(object sender, EventArgs e)
        {
            // use the sender to identify the dice
            Button currB = (Button)sender;

            UpdateDiceColours(currB);


        }

        private void BtnNumberScores_Clicked(object sender, EventArgs e)
        {
            // calculate scores for 1s, 2s, 3s, etc
            // use thesender object
            ImageButton currB = (ImageButton)sender;
            string valueScore = currB.StyleId;

            //HOLD THE SCORE
            int Index = Convert.ToInt32(valueScore);
            KeepScore[Index - 1] = true;

            // count all the 1s on the dice
            int total = CalculateScoresForNumbers(valueScore);


            string UpdateLabel = "Lbl" + currB.StyleId;
            Label l = (Label)FindByName(UpdateLabel);   // searches the x:Name= fields
            l.Text = total.ToString();

            currB.IsEnabled = false;


            BtnDiceRoll.IsEnabled = true;
            // currB.StyleId = "Roll";
            EndRound();



        }

        private void UpdateDiceColours(Button currB)
        {
            if (currB.StyleId == "Roll")
            {
                currB.StyleId = "Stick";
                currB.BackgroundColor = Color.Blue;
                currB.TextColor = Color.White;
            }
            else
            {
                currB.StyleId = "Roll";
                currB.BackgroundColor = Color.LightGray;
                currB.TextColor = Color.Black;
            }
        }


        private void NewGame()
        {
            KeepScore = new bool[13];

            ///////////////////////////////SETTING UP FOR NEW GAME/////////////////////////////////////////
            Set3 = 0;
            Set4 = 0;
            SetFull = 0;
            SetSmall = 0;
            SetLong = 0;
            SetYahtzee = 0;
            SetChance = 0;
            SetYahBonus = 0;

            ones = 0;
            twos = 0;
            threes = 0;
            fours = 0;
            fives = 0;
            sixs = 0;

            Lbl1.Text = "0";
            Lbl2.Text = "0";
            Lbl3.Text = "0";
            Lbl4.Text = "0";
            Lbl5.Text = "0";
            Lbl6.Text = "0";

            LblUpperBaseScore.Text = "0";
            bonusLbl.Text = "0";
            LblTotalUp.Text = "0";
            LblBtn3OfKind.Text = "0";
            LblBtn4OfAKind.Text = "0";
            LblFullHouse.Text = "0";
            LblSmall.Text = "0";
            LblLong.Text = "0";
            LblYahtzee.Text = "0";
            LblChance.Text = "0";
            LblYahBonus.Text = "0";

            LblTotalUp2.Text = "0";
            LblBottomBaseScore.Text = "0";
            LblGrandTotal.Text = "0";



            stopYah = false;



            EndRound();
            LblFeedback.Text = "Roll Dice to begin";
        }

        private async void EndRound()
        {
            BtnDiceRoll.IsEnabled = true;
            _rollNumber = 1;

            // have to stop user from holding onto buttons from first 3 rolls
            BtnDice1.StyleId = "Stick";
            BtnDice2.StyleId = "Stick";
            BtnDice3.StyleId = "Stick";
            BtnDice4.StyleId = "Stick";
            BtnDice5.StyleId = "Stick";

            BtnOnes.IsEnabled = false;
            BtnTwos.IsEnabled = false;
            BtnThrees.IsEnabled = false;
            BtnFours.IsEnabled = false;
            BtnFives.IsEnabled = false;
            BtnSixes.IsEnabled = false;
            threeOfAKindBtn.IsEnabled = false;
            fourOfAKindBtn.IsEnabled = false;
            fullHouseBtn.IsEnabled = false;
            smallBtn.IsEnabled = false;
            longBtn.IsEnabled = false;
            YahtzeeBtn.IsEnabled = false;
            ChanceBtn.IsEnabled = false;



            //DISABLE ALL Dice
            BtnDice1.IsEnabled = false;
            BtnDice2.IsEnabled = false;
            BtnDice3.IsEnabled = false;
            BtnDice4.IsEnabled = false;
            BtnDice5.IsEnabled = false;

            UpdateDiceColours(BtnDice1);
            UpdateDiceColours(BtnDice2);
            UpdateDiceColours(BtnDice3);
            UpdateDiceColours(BtnDice4);
            UpdateDiceColours(BtnDice5);


            /////////////////////-----------Checking for a yahtzee bonus--------////////////////////////////

            //stop from doing yahtzee bonus straight away
            
                int Dice1, Dice2, Dice3, Dice4, Dice5;

                //Converting these to Ints from Strings
                Dice1 = Convert.ToInt32(BtnDice1.Text);
                Dice2 = Convert.ToInt32(BtnDice2.Text);
                Dice3 = Convert.ToInt32(BtnDice3.Text);
                Dice4 = Convert.ToInt32(BtnDice4.Text);
                Dice5 = Convert.ToInt32(BtnDice5.Text);


                int check = 0;

                check += Convert.ToInt32(LblYahtzee.Text);

                if (check != 0)
                {
                    if (Dice1 == Dice2 && Dice2 == Dice3 && Dice3 == Dice4 && Dice4 == Dice5)
                    {
                        if(stopYah == true) //Stop from doing yahtzee bonus staright away
                        {
                            //Set Score
                            SetYahBonus = 100;
                        }
                       
                    }
                    stopYah = true;

                    //Display Score
                    LblYahBonus.Text = SetYahBonus.ToString();
                }

            
            

            UpdateUpperScore();
            UpdateLowerScore();

            bool endGame = true;

            //making sure all scores are over 0
            for (int i = 0; i < KeepScore.Length; i++)
            {
                if (KeepScore[i] == false)
                {
                    endGame = false;

                }
            }
            if (endGame == true)
            {
                await DisplayAlert("Well Done!!! \n You scored " + LblGrandTotal.Text + " \n  New Game", "Are you sure you want to start a new Game?", "New Game", "Cancel");
                NewGame();
            }



        }

        private int CalculateScoresForNumbers(string ScoreNumber)
        {
            int total = 0;
            // check all the dice
            if (BtnDice1.Text == ScoreNumber) total += Convert.ToInt32(BtnDice1.Text);
            if (BtnDice2.Text == ScoreNumber) total += Convert.ToInt32(BtnDice2.Text);
            if (BtnDice3.Text == ScoreNumber) total += Convert.ToInt32(BtnDice3.Text);
            if (BtnDice4.Text == ScoreNumber) total += Convert.ToInt32(BtnDice4.Text);
            if (BtnDice5.Text == ScoreNumber) total += Convert.ToInt32(BtnDice5.Text);

            return total;

        }

        private void CalculatePredictedScores()
        {
            int i;
            int total;
            string feedback = " PICK A SCORE AND ROLL AGAIN\n Options: ";

            for (i = 1; i < 7; i++)
            {
                total = CalculateScoresForNumbers(i.ToString());
                feedback += i.ToString() + " - " + total.ToString() + ", ";
            }

            // check for yahtzee
            if (BtnDice1.Text == BtnDice2.Text &&
                BtnDice1.Text == BtnDice3.Text &&
                BtnDice1.Text == BtnDice4.Text &&
                BtnDice1.Text == BtnDice5.Text)
            {
                feedback += "Yahtzee!";
            }




            LblFeedback.TextColor = Color.Blue;
            LblFeedback.Text = feedback;


        }


        private void UpdateUpperScore()
        {
            string labelName;
            Label l;
            int totalValue = 0;
            int bonus = 35;


            for (int i = 1; i < 7; i++)
            {
                labelName = "Lbl" + i.ToString();   // Lbl1 x:Name="Lbl1"
                l = (Label)FindByName(labelName);
                if (l.Text != "") // set as const string NO_SCORE_STR = "---";
                {
                    totalValue += Convert.ToInt32(l.Text);
                }
            }

            // update something on the screen - label
            LblUpperBaseScore.Text = totalValue.ToString();

            // check if > 63 then give the bonus of 35 and display in the 
            // Upper Final Score label

            if (totalValue > 63)
            {
                bonusLbl.Text = bonus.ToString();
                totalValue += bonus;
            }

            else
            {
                bonusLbl.Text = "0";
            }


        }
        private void UpdateLowerScore()
        {
            int totalValue = 0;

            // Change the text to an Int
            totalValue += Convert.ToInt32(LblBtn3OfKind.Text);
            totalValue += Convert.ToInt32(LblBtn4OfAKind.Text);
            totalValue += Convert.ToInt32(LblFullHouse.Text);
            totalValue += Convert.ToInt32(LblSmall.Text);
            totalValue += Convert.ToInt32(LblLong.Text);
            totalValue += Convert.ToInt32(LblYahtzee.Text);
            totalValue += Convert.ToInt32(LblChance.Text);
            totalValue += Convert.ToInt32(LblYahBonus.Text);

            // Update something on the screen - label
            LblBottomBaseScore.Text = totalValue.ToString();

            // Adding the bonus to Total Upper Score
            int UpperScoreTotal = Convert.ToInt32(LblUpperBaseScore.Text);
            int BonusScore = Convert.ToInt32(bonusLbl.Text);

            LblTotalUp.Text = (UpperScoreTotal + BonusScore).ToString();
            LblTotalUp2.Text = (UpperScoreTotal + BonusScore).ToString();

            LblGrandTotal.Text = (UpperScoreTotal + totalValue).ToString();
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //----------------------------------------------------------------------------------BOTTTOM OF THE TABLE-------------------------------------------------------------------------------------
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //-------------------------------------- THREE OF A KIND -------------------------------------- //
        private void threeOfAKindBtn_Clicked(object sender, EventArgs e)
        {
            ImageButton currB = (ImageButton)sender;
            string valueScore = currB.StyleId;

            //HOLD THE SCORE
            int Index = Convert.ToInt32(valueScore);
            KeepScore[Index - 1] = true;

            int Dice1, Dice2, Dice3, Dice4, Dice5;

            //Converting these to Ints from Strings
            Dice1 = Convert.ToInt32(BtnDice1.Text);
            Dice2 = Convert.ToInt32(BtnDice2.Text);
            Dice3 = Convert.ToInt32(BtnDice3.Text);
            Dice4 = Convert.ToInt32(BtnDice4.Text);
            Dice5 = Convert.ToInt32(BtnDice5.Text);



            //Check if you have a 3 of kind
            if (Dice1 == Dice2 && Dice2 == Dice3 || Dice1 == Dice2 && Dice2 == Dice4 ||
                    Dice1 == Dice2 && Dice2 == Dice5 || Dice1 == Dice3 && Dice3 == Dice4 ||
                    Dice1 == Dice3 && Dice3 == Dice5 || Dice1 == Dice4 && Dice4 == Dice5 ||
                    Dice2 == Dice3 && Dice3 == Dice4 || Dice2 == Dice3 && Dice3 == Dice5 ||
                    Dice2 == Dice4 && Dice4 == Dice5 || Dice3 == Dice4 && Dice4 == Dice5)
            {
                //Set Score
                Set3 = Dice1 + Dice2 + Dice3 + Dice4 + Dice5;


            }

            //Display Score
            LblBtn3OfKind.Text = Set3.ToString();
            EndRound();
        }

        //-------------------------------------- FOUR OF A KIND --------------------------------------//
        private void fourOfAKindBtn_Clicked(object sender, EventArgs e)
        {
            ImageButton currB = (ImageButton)sender;
            string valueScore = currB.StyleId;

            //HOLD THE SCORE
            int Index = Convert.ToInt32(valueScore);
            KeepScore[Index - 1] = true;

            int Dice1, Dice2, Dice3, Dice4, Dice5;

            //Converting these to Ints from Strings
            Dice1 = Convert.ToInt32(BtnDice1.Text);
            Dice2 = Convert.ToInt32(BtnDice2.Text);
            Dice3 = Convert.ToInt32(BtnDice3.Text);
            Dice4 = Convert.ToInt32(BtnDice4.Text);
            Dice5 = Convert.ToInt32(BtnDice5.Text);

            if (Dice1 == Dice2 && Dice2 == Dice3 && Dice3 == Dice4
                || Dice1 == Dice2 && Dice2 == Dice3 && Dice3 == Dice5
                || Dice2 == Dice3 && Dice3 == Dice4 && Dice4 == Dice5
                || Dice1 == Dice2 && Dice2 == Dice4 && Dice4 == Dice5)
            {
                //Set Score
                Set4 = Dice1 + Dice2 + Dice3 + Dice4 + Dice5;

            }

            //Display Score
            LblBtn4OfAKind.Text = Set4.ToString();
            EndRound();

            int sum = 0;
            bool FourOfAKind = false;

            for (int i = 1; i <= 6; i++)
            {
                int Count = 0;


                //Converting Dice to Ints from Strings
                if (Convert.ToInt32(BtnDice1.Text) == i)
                {
                    Count++;
                }

                if (Convert.ToInt32(BtnDice2.Text) == i)
                {
                    Count++;
                }

                if (Convert.ToInt32(BtnDice3.Text) == i)
                {
                    Count++;
                }

                if (Convert.ToInt32(BtnDice4.Text) == i)
                {
                    Count++;
                }

                if (Convert.ToInt32(BtnDice5.Text) == i)
                {
                    Count++;
                }

                if (Count > 3)
                  FourOfAKind = true;
            
            }

                if (FourOfAKind)
                {
                    //Set Score
                    Set4 = Dice1 + Dice2 + Dice3 + Dice4 + Dice5;
                }


            }

        //--------------------------------------FULL HOUSE--------------------------------------//
        private void fullHouseBtn_Clicked(object sender, EventArgs e)
        {

            ImageButton currB = (ImageButton)sender;
            string valueScore = currB.StyleId;

            //HOLD THE SCORE
            int Index = Convert.ToInt32(valueScore);
            KeepScore[Index - 1] = true;

            int Dice1, Dice2, Dice3, Dice4, Dice5;

            //Converting these to Ints from Strings
            Dice1 = Convert.ToInt32(BtnDice1.Text);
            Dice2 = Convert.ToInt32(BtnDice2.Text);
            Dice3 = Convert.ToInt32(BtnDice3.Text);
            Dice4 = Convert.ToInt32(BtnDice4.Text);
            Dice5 = Convert.ToInt32(BtnDice5.Text);

            //Check if conditions met
            if (Dice1 == Dice2 && Dice2 == Dice3 && Dice4 == Dice5 || Dice1 == Dice2 && Dice2 == Dice4 && Dice3 == Dice5 ||
                    Dice1 == Dice2 && Dice2 == Dice5 && Dice3 == Dice4 || Dice1 == Dice3 && Dice3 == Dice4 && Dice2 == Dice5 ||
                    Dice1 == Dice3 && Dice3 == Dice5 && Dice2 == Dice4 || Dice1 == Dice4 && Dice4 == Dice5 && Dice2 == Dice3 ||
                    Dice2 == Dice3 && Dice3 == Dice4 && Dice1 == Dice5 || Dice2 == Dice3 && Dice3 == Dice5 && Dice1 == Dice4 ||
                    Dice2 == Dice4 && Dice4 == Dice5 && Dice1 == Dice3 || Dice3 == Dice4 && Dice4 == Dice5 && Dice1 == Dice2)
            {
                //Set Score
                SetFull = 25;
            }
            //Display Score
            LblFullHouse.Text = SetFull.ToString();
            EndRound();
        }



        //--------------------------------------SMALL STRAIGHT--------------------------------------//
        private void smallBtn_Clicked(object sender, EventArgs e)
        {

            ImageButton currB = (ImageButton)sender;
            string valueScore = currB.StyleId;

            //HOLD THE SCORE
            int Index = Convert.ToInt32(valueScore);
            KeepScore[Index - 1] = true;/////////////////////////////////////////////////ADD IN 

            int Dice1, Dice2, Dice3, Dice4, Dice5;

            //Converting these to Ints from Strings
            Dice1 = Convert.ToInt32(BtnDice1.Text);
            Dice2 = Convert.ToInt32(BtnDice2.Text);
            Dice3 = Convert.ToInt32(BtnDice3.Text);
            Dice4 = Convert.ToInt32(BtnDice4.Text);
            Dice5 = Convert.ToInt32(BtnDice5.Text);

            //IF DICE 1234 IS THE SAME
            if (Dice1 == 1 || Dice2 == 1 || Dice3 == 1 || Dice4 == 1 || Dice5 == 1)
            {
                if (Dice1 == 2 || Dice2 == 2 || Dice3 == 2 || Dice4 == 2 || Dice5 == 2)
                {
                    if (Dice1 == 3 || Dice2 == 3 || Dice3 == 3 || Dice4 == 3 || Dice5 == 3)
                    {
                        if (Dice1 == 4 || Dice2 == 4 || Dice3 == 4 || Dice4 == 4 || Dice5 == 4)
                        {


                            //Set Score
                            SetSmall = 30;


                        }
                    }
                }
            }

            // IF DICE 2345 IS THE SAME
            if (Dice1 == 2 || Dice2 == 2 || Dice3 == 2 || Dice4 == 2 || Dice5 == 2)
            {
                if (Dice1 == 3 || Dice2 == 3 || Dice3 == 3 || Dice4 == 3 || Dice5 == 3)
                {
                    if (Dice1 == 4 || Dice2 == 4 || Dice3 == 4 || Dice4 == 4 || Dice5 == 4)
                    {
                        if (Dice1 == 5 || Dice2 == 5 || Dice3 == 5 || Dice4 == 5 || Dice5 == 5)
                        {


                            //Set Score
                            SetSmall = 25;
                        }
                    }
                }
            }

            //IF DICE 3456 IS THE SAME
            if (Dice1 == 3 || Dice2 == 3 || Dice3 == 3 || Dice4 == 3 || Dice5 == 3)
            {
                if (Dice1 == 4 || Dice2 == 4 || Dice3 == 4 || Dice4 == 4 || Dice5 == 4)
                {
                    if (Dice1 == 5 || Dice2 == 5 || Dice3 == 5 || Dice4 == 5 || Dice5 == 5)
                    {
                        if (Dice1 == 6 || Dice2 == 6 || Dice3 == 6 || Dice4 == 6 || Dice5 == 6)
                        {


                            //Set Score
                            SetSmall = 30;
                        }
                    }
                }
            }



            //Display score if the conditons are met
            LblSmall.Text = SetSmall.ToString();
            EndRound();




        }//END OF SMALL STR

        //-------------------------------------Long Straight---------------------------------------//
        private void longBtn_Clicked(object sender, EventArgs e)
        {
            ImageButton currB = (ImageButton)sender;
            string valueScore = currB.StyleId;

            //HOLD THE SCORE
            int Index = Convert.ToInt32(valueScore);
            KeepScore[Index - 1] = true;/////////////////////////////////////////////////ADD IN 

            int Dice1, Dice2, Dice3, Dice4, Dice5;

            //Converting these to Ints from Strings
            Dice1 = Convert.ToInt32(BtnDice1.Text);
            Dice2 = Convert.ToInt32(BtnDice2.Text);
            Dice3 = Convert.ToInt32(BtnDice3.Text);
            Dice4 = Convert.ToInt32(BtnDice4.Text);
            Dice5 = Convert.ToInt32(BtnDice5.Text);

            //Checking how many ones, Twos .. ect

            if (Dice1 == 1)
                ones += 1;
            if (Dice1 == 2)
                twos += 1;
            if (Dice1 == 3)
                threes += 1;
            if (Dice1 == 4)
                fours += 1;
            if (Dice1 == 5)
                fives += 1;
            if (Dice1 == 6)
                sixs += 1;

            if (Dice2 == 1)
                ones += 1;
            if (Dice2 == 2)
                twos += 1;
            if (Dice2 == 3)
                threes += 1;
            if (Dice2 == 4)
                fours += 1;
            if (Dice2 == 5)
                fives += 1;
            if (Dice2 == 6)
                sixs += 1;

            if (Dice3 == 1)
                ones += 1;
            if (Dice3 == 2)
                twos += 1;
            if (Dice3 == 3)
                threes += 1;
            if (Dice3 == 4)
                fours += 1;
            if (Dice3 == 5)
                fives += 1;
            if (Dice3 == 6)
                sixs += 1;

            if (Dice4 == 1)
                ones += 1;
            if (Dice4 == 2)
                twos += 1;
            if (Dice4 == 3)
                threes += 1;
            if (Dice4 == 4)
                fours += 1;
            if (Dice4 == 5)
                fives += 1;
            if (Dice4 == 6)
                sixs += 1;

            if (Dice5 == 1)
                ones += 1;
            if (Dice5 == 2)
                twos += 1;
            if (Dice5 == 3)
                threes += 1;
            if (Dice5 == 4)
                fours += 1;
            if (Dice5 == 5)
                fives += 1;
            if (Dice5 == 6)
                sixs += 1;


            if (ones == 1 && twos == 1 && threes == 1 && fours == 1 && fives == 1 ||
                twos == 1 && threes == 1 && fours == 1 && fives == 1 && sixs == 1)

            {
                //Set Score
                SetLong = 40;


            }
            //Display Score
            LblLong.Text = SetLong.ToString();
            EndRound();
        }//------------------------------------End of Long Str------------------------------//


        //--------------------------------------Yahtzee--------------------------------------//
        private void YahtzeeBtn_Clicked(object sender, EventArgs e)
        {
            ImageButton currB = (ImageButton)sender;
            string valueScore = currB.StyleId;

            //HOLD THE SCORE
            int Index = Convert.ToInt32(valueScore);
            KeepScore[Index - 1] = true;

            int Dice1, Dice2, Dice3, Dice4, Dice5;

            //Converting these to Ints from Strings
            Dice1 = Convert.ToInt32(BtnDice1.Text);
            Dice2 = Convert.ToInt32(BtnDice2.Text);
            Dice3 = Convert.ToInt32(BtnDice3.Text);
            Dice4 = Convert.ToInt32(BtnDice4.Text);
            Dice5 = Convert.ToInt32(BtnDice5.Text);

            if (Dice1 == Dice2 && Dice2 == Dice3 && Dice3 == Dice4 && Dice4 == Dice5)
            {

                //Set Score
                SetYahtzee = 50;
            }

            //Display Score
            LblYahtzee.Text = SetYahtzee.ToString();
            EndRound();

        }



        //-------------------------------------- Chance --------------------------------------//
        private void ChanceBtn_Clicked(object sender, EventArgs e)
        {
            ImageButton currB = (ImageButton)sender;
            string valueScore = currB.StyleId;

            //HOLD THE SCORE
            int Index = Convert.ToInt32(valueScore);
            KeepScore[Index - 1] = true;

            int Dice1, Dice2, Dice3, Dice4, Dice5;

            //Converting these to Ints from Strings
            Dice1 = Convert.ToInt32(BtnDice1.Text);
            Dice2 = Convert.ToInt32(BtnDice2.Text);
            Dice3 = Convert.ToInt32(BtnDice3.Text);
            Dice4 = Convert.ToInt32(BtnDice4.Text);
            Dice5 = Convert.ToInt32(BtnDice5.Text);

            //Set Score
            SetChance = Dice1 + Dice2 + Dice3 + Dice4;

            //Display Score
            LblChance.Text = SetChance.ToString();

            EndRound();

        }


        //-------------------------------------- Restarting Game --------------------------------------//
        private async void BtnRestartGame_clicked(object sender, EventArgs e)
        {
            bool newGame = await DisplayAlert("New Game", "Are you sure you want to start a new Game?", "New Game", "Cancel");
            if (newGame)
            {
                NewGame();
            }

        }


    }
}

