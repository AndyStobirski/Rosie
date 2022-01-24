using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rosie
{
    /// <summary>
    /// Sample class for animation - creat
    /// </summary>
    public class AnimationEffect
    {
        public AnimationEffect(SpriteFont pFont, string pText, float pX, float pY)
        {
            _font = pFont;
            _text = pText;
            _y = pY;
            _x = pX;
        }
        public bool Remove { get {return Life ==0; }  }
        public int Life { get; set; } = 30;

        private SpriteFont _font;        
        private string _text;
        float _y;
        float _x;
        public void Tick()
        {
            Life--;
            _y -= 2f;
        }
        public void Draw (SpriteBatch _sb)
        {
            _sb.DrawString(_font, _text, new Vector2(_x, _y), Color.Red);
        }

    }
}
