using Rosie.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rosie.Misc
{

    /// <summary>
    /// Each actor has a Speed statistic which will represent how often they can take actions,
    /// which means that actors move in relation to one another
    /// 
    /// So imagine Goblin speed = 2, Player speed = 4, Ooze = 8 and thy move as follows
    /// 
    /// 1   2   3   4   5   6   7   8   9   10  11  12  13  14  15  16
    ///     G       G       G       G       G       G       G       G
    ///             P               P               P               P
    ///                             O                               O
    /// 
    /// <notQuiteCorrect>
    /// So all we need to do take the maximum speed value (i.e. the slowest) and count 
    /// upwards it from 1 and check the modulus of the counter against speed of each 
    /// actor, if that value is 0 (i.e. no remainder) it means the actor moves.
    /// </notQuiteCorrect>
    /// 
    /// The above only works for collections of event numbers, if you had collections
    /// of odd and evens, or just odds, all the unique speeds need to multiplied to ensure
    /// a cycle of movement, so you start counting from one up to the the maximum number
    /// and then reset back to 1 when it is reached
    /// 
    /// e.g. speeds g2,p3,o4
    /// 
    /// 1   2   3   4   5   6   7   8   9   10  11  12  13  14  15  16  17  18  19  20  21  22  23  24
    ///     g       g       g       g       g       g       g       g       g       g       g       g
    ///         p           p           p           p           p           p           p           p
    ///             o               o               o               o               o               o
    /// </summary>
    public class Scheduler
    {

        private List<Actor> _actors = new List<Actor>();
        private int _position = 0;
        private int _maxSpeed = 0;

        public void AddActor(Actor pActor)
        {
            _actors.Add(pActor);
            _maxSpeed = _actors
                            .Select(a => a.Speed)
                            .Distinct()
                            .Aggregate((x, y) => x * y);
        }


        public void RemoveActor(Actor pActor)
        {
            _actors.Remove(pActor);
            _maxSpeed = _actors
                            .Select(a => a.Speed)
                            .Distinct()
                            .Aggregate((x, y) => x * y);
        }

        /// <summary>
        /// Actors whose speed with mod of 0 get to move on this current tick
        /// </summary>
        /// <returns>List of actors whose turn it is to move</returns>
        public IEnumerable<Actor> Tick()
        {

            IEnumerable<Actor> ret = null;
            do
            {
                _position++;
                ret = _actors.Where(i => _position % i.Speed == 0);

            } while (!ret.Any());

            if (_position == _maxSpeed)
                _position = 0;

            return ret;
        }

        public void Reset()
        {
            _actors = new List<Actor>();
        }

    }
}
