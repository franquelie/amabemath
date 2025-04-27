package com.amabe.math.gameobject;

import android.graphics.Canvas;

import com.amabe.math.GameDisplay;

/**
 * GameObject is an abstract class which is the foundation of all world objects in the game.
 * Player sprite revert to moving around.
 */
public abstract class GameObject {
    protected double positionX = 0;
    protected double positionY = 0;
    protected double velocityX = 0;
    protected double velocityY = 0;
    protected double directionX = 1;
    protected double directionY = 0;

    public GameObject(double positionX, double positionY) {
        this.positionX = positionX;
        this.positionY = positionY;
    }

    public double getPositionX() { return positionX; }
    public double getPositionY() { return positionY; }

    protected double getDirectionX() { return directionX; }
    protected double getDirectionY() { return directionY; }

    public abstract void draw(Canvas canvas); // Draw method for player moving around the screen
    public abstract void draw(Canvas canvas, GameDisplay gameDisplay); // Draw method for player in the middle of the screen
    public abstract void update();

    protected static double getDistanceBetweenObjects(GameObject obj1, GameObject obj2) {
        return Math.sqrt(
                Math.pow(obj2.getPositionX() - obj1.getPositionX(), 2) +
                Math.pow(obj2.getPositionY() - obj1.getPositionY(), 2)
        );
    }

}
