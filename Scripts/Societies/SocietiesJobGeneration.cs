using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Server;
using Server.Items;
using Server.Commands;
using Server.Mobiles;
using Server.Gumps;
using Server.Accounting;

namespace Server
{
    public static class SocietiesJobGeneration
    {
        public static void GenerateJobs(SocietiesGroupType societyGroupType)
        {
            SocietyJob societyJob = null;

            switch (societyGroupType)
            {
                #region Adventurer's Lodge

                case SocietiesGroupType.AdventurersLodge:
                break;

                #endregion

                #region Artificer's Enclave

                case SocietiesGroupType.ArtificersEnclave:
                    //Alchemy Easy
                    societyJob = new SocietyJob();
                    societyJob.m_SocietiesGroupType = SocietiesGroupType.ArtificersEnclave;
                    societyJob.m_JobType = SocietyJob.JobType.CraftItem;
                    societyJob.m_Expiration = Societies.NextJobsAdded + TimeSpan.FromHours(24);
                    societyJob.m_PointValue = 2;
                    societyJob.m_DestinationMobile = typeof(Alchemist);
                    societyJob.m_DestinationMobileName = "Alchemist";
                    societyJob.m_DestinationTown = Towns.GetRandomTown();

                    #region Potential Jobs
                    switch (Utility.RandomMinMax(1, 1))
                    {
                        case 1:                            
                            societyJob.m_PrimaryType = typeof(GreaterHealPotion);
                            societyJob.m_PrimaryTypeName = "Greater Heal Potions";
                            societyJob.m_TargetNumber = Utility.RandomList(350, 375, 400, 425, 450);
                            societyJob.m_TurnInRequirementAmount = societyJob.m_TargetNumber;
                            societyJob.m_IconItemId = 3852;
                            societyJob.m_IconOffsetX = 55;
                            societyJob.m_IconOffsetY = 40;
                        break;
                    }
                    #endregion

                    Societies.m_SocietyJobs.Add(societyJob);

                    //Alchemy Hard
                    societyJob = new SocietyJob();
                    societyJob.m_SocietiesGroupType = SocietiesGroupType.ArtificersEnclave;
                    societyJob.m_JobType = SocietyJob.JobType.CraftItem;
                    societyJob.m_Expiration = Societies.NextJobsAdded + TimeSpan.FromHours(24);
                    societyJob.m_PointValue = 4;
                    societyJob.m_DestinationMobile = typeof(Alchemist);
                    societyJob.m_DestinationMobileName = "Alchemist";
                    societyJob.m_DestinationTown = Towns.GetRandomTown();

                    #region Potential Jobs
                    switch (Utility.RandomMinMax(1, 1))
                    {
                        case 1:
                            societyJob.m_PrimaryType = typeof(LethalPoisonPotion);
                            societyJob.m_PrimaryTypeName = "Lethal Poison Potions";
                            societyJob.m_TargetNumber = Utility.RandomList(40, 45, 50, 55, 60);
                            societyJob.m_TurnInRequirementAmount = societyJob.m_TargetNumber;
                            societyJob.m_IconItemId = 3850;
                            societyJob.m_IconOffsetX = 55;
                            societyJob.m_IconOffsetY = 40;
                        break;
                    }
                    #endregion

                    Societies.m_SocietyJobs.Add(societyJob);

                    //-----

                    //Inscription Easy
                    societyJob = new SocietyJob();
                    societyJob.m_SocietiesGroupType = SocietiesGroupType.ArtificersEnclave;
                    societyJob.m_JobType = SocietyJob.JobType.CraftItem;
                    societyJob.m_Expiration = Societies.NextJobsAdded + TimeSpan.FromHours(24);
                    societyJob.m_PointValue = 2;
                    societyJob.m_DestinationMobile = typeof(Scribe);
                    societyJob.m_DestinationMobileName = "Scribe";
                    societyJob.m_DestinationTown = Towns.GetRandomTown();

                    #region Potential Jobs
                    switch (Utility.RandomMinMax(1, 1))
                    {
                        case 1:
                            SpellScroll spellScroll = SpellScroll.GetRandomSpellScroll(6);

                            if (spellScroll == null)
                                return;

                            societyJob.m_PrimaryType = spellScroll.GetType();
                            societyJob.m_PrimaryTypeName = Utility.Capitalize(spellScroll.Name) + "s";
                            societyJob.m_TargetNumber = Utility.RandomList(150, 175, 200, 225, 250);
                            societyJob.m_TurnInRequirementAmount = societyJob.m_TargetNumber;
                            societyJob.m_IconItemId = spellScroll.IconItemId;
                            societyJob.m_IconHue = spellScroll.IconHue;
                            societyJob.m_IconOffsetX = spellScroll.IconOffsetX;
                            societyJob.m_IconOffsetY = spellScroll.IconOffsetY;

                            spellScroll.Delete();
                        break;

                    }
                    #endregion

                    Societies.m_SocietyJobs.Add(societyJob);

                    //Inscription Hard
                    societyJob = new SocietyJob();
                    societyJob.m_SocietiesGroupType = SocietiesGroupType.ArtificersEnclave;
                    societyJob.m_JobType = SocietyJob.JobType.CraftItem;
                    societyJob.m_Expiration = Societies.NextJobsAdded + TimeSpan.FromHours(24);
                    societyJob.m_PointValue = 4;
                    societyJob.m_DestinationMobile = typeof(Scribe);
                    societyJob.m_DestinationMobileName = "Scribe";
                    societyJob.m_DestinationTown = Towns.GetRandomTown();

                    #region Potential Jobs
                    switch (Utility.RandomMinMax(1, 1))
                    {
                        case 1:
                            SpellScroll spellScroll = null;

                            double randomResult = Utility.RandomDouble();

                            if (randomResult <= .50)
                            {
                                spellScroll = SpellScroll.GetRandomSpellScroll(7);
                                societyJob.m_TargetNumber = Utility.RandomList(12, 14, 16, 18, 20);
                            }

                            else
                            {
                                spellScroll = SpellScroll.GetRandomSpellScroll(8);
                                societyJob.m_TargetNumber = Utility.RandomList(6, 7, 8, 9, 10);
                            }

                            if (spellScroll == null)
                                return;

                            societyJob.m_PrimaryType = spellScroll.GetType();
                            societyJob.m_PrimaryTypeName = Utility.Capitalize(spellScroll.Name) + "s";
                            
                            societyJob.m_TurnInRequirementAmount = societyJob.m_TargetNumber;
                            societyJob.m_IconItemId = spellScroll.IconItemId;
                            societyJob.m_IconHue = spellScroll.IconHue;
                            societyJob.m_IconOffsetX = spellScroll.IconOffsetX;
                            societyJob.m_IconOffsetY = spellScroll.IconOffsetY;

                            spellScroll.Delete();
                        break;
                    }
                    #endregion

                    Societies.m_SocietyJobs.Add(societyJob);
                break;

                #endregion

                #region Farmer's Cooperative

                case SocietiesGroupType.FarmersCooperative:
                break;

                #endregion

                #region Fisherman's Circle

                case SocietiesGroupType.FishermansCircle:
                break;

                #endregion

                #region Monster Hunter's Society

                case SocietiesGroupType.MonsterHuntersSociety:
                break;

                #endregion

                #region Seafarer's League

                case SocietiesGroupType.SeafarersLeague:
                break;

                #endregion

                #region Smithing Order

                case SocietiesGroupType.SmithingOrder:
                break;

                #endregion

                #region Thieves Guild

                case SocietiesGroupType.ThievesGuild:
                break;

                #endregion

                #region Tradesman Union

                case SocietiesGroupType.TradesmanUnion:
                break;

                #endregion

                #region Zoological Foundation

                case SocietiesGroupType.ZoologicalFoundation:
                break;

                #endregion
            }
        }
    }
}
