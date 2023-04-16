using Rosie.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rosie.Misc
{

    /// <summary>
    /// 
    /// https://roguesharp.wordpress.com/2016/08/27/roguesharp-v3-tutorial-scheduling-system/
    /// 
    /// Each actor has a Speed statistic which will represent how often they can take actions,
    /// which means that actors move in relation to one another
    /// 
    /// So imagine Goblin speed = 2, Player speed = 4, Ooze = 4 and they move as follows
    /// 
    /// 1   2   3   4   5   6   7   8   9   10  11  12  13  14  15  16  17  18  19  20  21  22  23  24
    ///     g       g       g       g       g       g       g       g       g       g       g       g
    ///         p           p           p           p           p           p           p           p
    ///             o               o               o               o               o               o
    /// 
    /// All the unique speeds need to multiplied to ensure a cycle of movement that covers all players, 
    /// so you start counting from one up to the the maximum number
    /// and then reset back to 1 when it is reached
    /// </summary>
    public class Scheduler
    {

        private List<Actor> _actors = new List<Actor>();
        private int _currentStep = 0;
        private int _maxStep = 0;

        /// <summary>
        /// Add an add actor into the actor list and set 
        /// </summary>
        /// <param name="pActor"></param>
        public void AddActor(Actor pActor)
        {
            _actors.Add(pActor);
            _maxStep = _actors
                            .Select(a => a.Speed)
                            .Distinct()
                            .Aggregate((x, y) => x * y);

        }

        /// <summary>
        /// Remove the specified actor from the list and recalculate the limit
        /// </summary>
        /// <param name="pActor"></param>
        public void RemoveActor(Actor pActor)
        {
            _actors.Remove(pActor);
            _maxStep = _actors
                            .Select(a => a.Speed)
                            .Distinct()
                            .Aggregate((x, y) => x * y);
        }

        public bool ContainsActor(Actor pActor) { return _actors.Contains(pActor); }

        /// <summary>
        /// Calculate the actors who's turn it is move. This code will always return
        /// a list of actors, as it will omit any steps that contain no moving actors
        /// </summary>
        /// <returns>List of actors whose turn it is to move</returns>
        public IEnumerable<Actor> Tick()
        {
            _currentStep++;
            //Actors whose speed with mod of 0 get to move on this current tick
            IEnumerable<Actor> ret = _actors.Where(i => _currentStep % i.Speed == 0);

            if (_currentStep == _maxStep)
                _currentStep = 0;

            return ret;
        }

        public void Reset()
        {
            _actors = new List<Actor>();
        }

    }
}
