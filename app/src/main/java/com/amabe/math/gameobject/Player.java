package com.amabe.math.gameobject;

import android.content.Context;
import android.graphics.Canvas;

import androidx.core.content.ContextCompat;

import com.amabe.math.GameDisplay;
import com.amabe.math.GameLoop;
import com.amabe.math.R;
import com.amabe.math.Utils;
import com.amabe.math.gamepanel.HealthBar;
import com.amabe.math.gamepanel.Joystick;
import com.amabe.math.graphics.Sprite;


/**
 * Player is the main character of the game, which the user can control with a touch joystick.
 * The player class is an extension of a Circle, which is an extension of a GameObject
 * The player sprite was reverted to moving around.
 */
public class Player extends Circle {
    public static final double SPEED_PIXELS_PER_SECOND = 400.0;
    public static final double MAX_SPEED = SPEED_PIXELS_PER_SECOND / GameLoop.MAX_UPS;
    public static final int MAX_HEALTH_POINTS = 10;
    private final Joystick joystick;
    private HealthBar healthBar;
    private int healthPoints;
    private Sprite sprite;

    public Player(Context context, Joystick joystick, double positionX, double positionY, double radius, Sprite sprite) {
        super(context, ContextCompat.getColor(context, R.color.player), positionX, positionY, radius);
        this.joystick = joystick;
        this.healthBar = new HealthBar(context, this);
        this.healthPoints = MAX_HEALTH_POINTS;
        this.sprite = sprite;
    }

    public void update() {
        // Update velocity based on actuator of joystick
        velocityX = joystick.getActuatorX()*MAX_SPEED;
        velocityY = joystick.getActuatorY()*MAX_SPEED;

        // Update position
        positionX += velocityX;
        positionY += velocityY;

        // Update direction
        if (velocityX != 0 || velocityY != 0) {
            // Normalize velocity to get direction (unit vector of velocity)
            double distance = Utils.getDistanceBetweenObjects(0, 0, velocityX, velocityY);
            directionX = velocityX / distance;
            directionY = velocityY / distance;

        }
    }

    // Draw method for player moving around the screen
    public void draw(Canvas canvas) {
        sprite.draw(canvas, (int) positionX - sprite.getWidth()/2, (int) positionY - sprite.getHeight()/2);
        healthBar.draw(canvas);
    }

    // Draw method for player in the middle of the screen
    public void draw(Canvas canvas, GameDisplay gameDisplay) {

        sprite.draw(
                canvas,
                (int) gameDisplay.gameToDisplayCoordinatesX(getPositionX()) - sprite.getWidth()/2,
                (int) gameDisplay.gameToDisplayCoordinatesY(getPositionY()) - sprite.getHeight()/2
                );
        healthBar.draw(canvas, gameDisplay);

    }

    public int getHealthPoints() {
        return healthPoints;
    }

    public void setHealthPoints(int healthPoints) {
        // Only allow positive values
        if (healthPoints >= 0) {
            this.healthPoints = healthPoints;
        }
    }
}
