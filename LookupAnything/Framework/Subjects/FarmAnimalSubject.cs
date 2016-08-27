using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pathoschild.LookupAnything.Framework.Constants;
using Pathoschild.LookupAnything.Framework.Fields;
using StardewValley;

namespace Pathoschild.LookupAnything.Framework.Subjects
{
    /// <summary>Describes a farm animal.</summary>
    public class FarmAnimalSubject : BaseSubject
    {
        /*********
        ** Properties
        *********/
        /// <summary>The underlying farm animal.</summary>
        private readonly FarmAnimal Animal;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="animal">The underlying farm animal.</param>
        /// <remarks>Reverse engineered from <see cref="FarmAnimal"/>.</remarks>
        public FarmAnimalSubject(FarmAnimal animal)
            : base(animal.name, null, animal.type)
        {
            this.Animal = animal;
            bool isFullyGrown = animal.age >= animal.ageWhenMature;
            this.AddCustomFields(
                new CharacterFriendshipField("Friendship", animal.friendshipTowardFarmer, Constant.AnimalFriendshipPointsPerLevel, Constant.AnimalFriendshipMaxPoints),
                new GenericField("Happiness", $"{Math.Round(animal.happiness / (byte.MaxValue * 1f) * 100)}%"),
                new GenericField("Mood today", animal.getMoodMessage()),
                new GenericField("Complaints", this.GetMoodReason(animal)),
                new GenericField("Petted today", animal.wasPet),
                new GenericField("Adult in", $"{animal.ageWhenMature - animal.age} days", hasValue: !isFullyGrown),
                new SaleValueField("Sells for", animal.getSellPrice(), 1),
                new GenericField("Produce quality", animal.currentProduce < 0 ? "not ready for harvest" : (animal.currentProduce == animal.defaultProduceIndex ? "normal" : "deluxe"), hasValue: isFullyGrown)
            );
        }

        /// <summary>Draw the subject portrait (if available).</summary>
        /// <param name="sprites">The sprite batch in which to draw.</param>
        /// <param name="position">The position at which to draw.</param>
        /// <param name="size">The size of the portrait to draw.</param>
        /// <returns>Returns <c>true</c> if a portrait was drawn, else <c>false</c>.</returns>
        public override bool DrawPortrait(SpriteBatch sprites, Vector2 position, Vector2 size)
        {
            this.Animal.Sprite.draw(sprites, position, 1, 0, 0, Color.White, scale: size.X / this.Animal.Sprite.getWidth());
            return true;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get a short explanation for the animal's current mod.</summary>
        /// <param name="animal">The farm animal.</param>
        private string GetMoodReason(FarmAnimal animal)
        {
            List<string> factors = new List<string>();

            // winter without heat
            if (Game1.IsWinter && Game1.currentLocation.numberOfObjectsWithName(StandardItem.Heater) <= 0)
                factors.Add("no heater in winter");

            // mood
            switch (animal.moodMessage)
            {
                case FarmAnimal.newHome:
                    factors.Add("moved into new home");
                    break;
                case FarmAnimal.hungry:
                    factors.Add("wasn't fed yesterday");
                    break;
                case FarmAnimal.disturbedByDog:
                    factors.Add($"was disturbed by {Game1.player.getPetName()}");
                    break;
                case FarmAnimal.leftOutAtNight:
                    factors.Add("was left outside last night");
                    break;
            }

            // not pet
            if (!animal.wasPet)
                factors.Add("hasn't been petted today");

            // return factors
            return string.Join(", ", factors);
        }
    }
}