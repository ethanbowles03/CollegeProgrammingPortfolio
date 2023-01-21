package game;

import java.awt.Graphics;
import java.awt.Point;
import java.awt.image.BufferedImage;

public class EffectOil implements Animatable {
	//Field
	
		private GameState state;
		private Point origin, dest;
		private double age; //How long it is alive
		private double x, y;
		private int dx, dy;
		
		public EffectOil(GameState state, Point origin, Point dest) {
			this.state = state;
			this.origin = origin;
			this.dest = dest;
			
			age = 0;
			x = origin.x;
			y = origin.y;
			dx = dest.x - origin.x;
			dy = dest.y - origin.y;
		}
		
		public void update(double timeElapsed) {
			age += timeElapsed;
			if(age > 0.25) {
				state.removeGameObject(this);
				return;
			}
			x += dx * 0.08; 
			y += dy * 0.08;
			
			Point p = new Point((int)x, (int)y);
			Enemy e = state.findNearestEnemy(p);
			
			if(e != null && e.getPosition().distance(p) < 18) {
				if ( e instanceof EnemyHaasTruck) {
					state.addScore(10);
				}else {
					state.addScore(5);
				}
				Hitmarker h = new Hitmarker(state,(int)e.getPosition().x,(int)e.getPosition().y);
				state.addGameObject(h);
				state.removeGameObject(e);
				state.removeGameObject(this);
			}
			
			
		}

		@Override
		public void draw(Graphics g, GameView v) {
			BufferedImage oilDrop = ResourceLoader.getLoader().getImage("WaterDrop.png");
			v.drawCenteredImage(g,"OilDrop.png", (int) x, (int) y, 1.0);
		}
}
