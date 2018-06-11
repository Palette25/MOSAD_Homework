#pragma once
#include "cocos2d.h"
using namespace cocos2d;

class HelloWorld : public cocos2d::Scene
{
public:
    static cocos2d::Scene* createScene();

    virtual bool init();
        
	void update(float tf) override;

	void move(int direction);

	void bloodChange(int num);

	void playAttack();

	void playDead();

	void hitByMonster(float sd);

	void createMonster(float sd);
    // implement the "static create()" method manually
    CREATE_FUNC(HelloWorld);
private:
	cocos2d::Sprite* player;
	cocos2d::Vector<SpriteFrame*> attack;
	cocos2d::Vector<SpriteFrame*> dead;
	cocos2d::Vector<SpriteFrame*> run;
	cocos2d::Vector<SpriteFrame*> idle;
	cocos2d::Size visibleSize;
	cocos2d::Vec2 origin;
	cocos2d::Label* time;
	cocos2d::Label* score;
	cocos2d::Label* max;
	int dtime;
	cocos2d::ProgressTimer* pT;
	bool live;
	char last_dir = 0;
};