#include "HelloWorldScene.h"
#include "SimpleAudioEngine.h"
#pragma execution_character_set("utf-8")

USING_NS_CC;

Scene* HelloWorld::createScene()
{
    return HelloWorld::create();
}

// Print useful error message instead of segfaulting when files are not there.
static void problemLoading(const char* filename)
{
    printf("Error while loading: %s\n", filename);
    printf("Depending on how you compiled you might have to add 'Resources/' in front of filenames in HelloWorldScene.cpp\n");
}

// on "init" you need to initialize your instance
bool HelloWorld::init()
{
    //////////////////////////////
    // 1. super init first
    if ( !Scene::init() )
    {
        return false;
    }
	schedule(schedule_selector(HelloWorld::update), 1.0f, kRepeatForever, 0);

    visibleSize = Director::getInstance()->getVisibleSize();
    origin = Director::getInstance()->getVisibleOrigin();
	//创建WASD按钮Label,以及XY按钮
	auto wLabel = Label::createWithTTF("W", "fonts/arial.ttf", 36);
	auto aLabel = Label::createWithTTF("A", "fonts/arial.ttf", 36);
	auto sLabel = Label::createWithTTF("S", "fonts/arial.ttf", 36);
	auto dLabel = Label::createWithTTF("D", "fonts/arial.ttf", 36);
	auto xLabel = Label::createWithTTF("X", "fonts/arial.ttf", 36);
	auto yLabel = Label::createWithTTF("Y", "fonts/arial.ttf", 36);
	auto time = Label::createWithTTF("180", "fonts/arial.ttf", 36);
	time->setPosition(Vec2(350, 380));
	time->setName("time");
	this->addChild(time, 1);

	//对应地创建各项MenuItem
	auto wItem = MenuItemLabel::create(wLabel, CC_CALLBACK_0(HelloWorld::move, this, 1));
	auto aItem = MenuItemLabel::create(aLabel, CC_CALLBACK_0(HelloWorld::move, this, 2));
	auto sItem = MenuItemLabel::create(sLabel, CC_CALLBACK_0(HelloWorld::move, this, 3));
	auto dItem = MenuItemLabel::create(dLabel, CC_CALLBACK_0(HelloWorld::move, this, 4));
	auto yItem = MenuItemLabel::create(yLabel, CC_CALLBACK_0(HelloWorld::playAttack, this));
	auto xItem = MenuItemLabel::create(xLabel, CC_CALLBACK_0(HelloWorld::playDead, this));

	//放置各项MenuItem，并生成对应的Menu
	wItem->setPosition(Vec2(80, 80));
	aItem->setPosition(Vec2(30, 30));
	sItem->setPosition(Vec2(80, 30));
	dItem->setPosition(Vec2(130, 30));
	xItem->setPosition(Vec2(visibleSize.width - 30, 80));
	yItem->setPosition(Vec2(visibleSize.width - 50, 30));

	auto menu = Menu::create(wItem, aItem, sItem, dItem, xItem, yItem,  NULL);
	menu->setPosition(Vec2::ZERO);
	this->addChild(menu, 1);

	//创建一张贴图
	auto texture = Director::getInstance()->getTextureCache()->addImage("$lucia_2.png");
	//从贴图中以像素单位切割，创建关键帧
	auto frame0 = SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(0, 0, 113, 113)));
	//使用第一帧创建精灵
	player = Sprite::createWithSpriteFrame(frame0);
	player->setName("player");
	player->setPosition(Vec2(origin.x + visibleSize.width / 2,
		origin.y + visibleSize.height / 2));
	addChild(player, 3);

	//hp条
	Sprite* sp0 = Sprite::create("hp.png", CC_RECT_PIXELS_TO_POINTS(Rect(0, 320, 420, 47)));
	Sprite* sp = Sprite::create("hp.png", CC_RECT_PIXELS_TO_POINTS(Rect(610, 362, 4, 16)));
	sp->setName("sp");

	//使用hp条设置progressBar
	pT = ProgressTimer::create(sp);
	pT->setScaleX(90);
	pT->setAnchorPoint(Vec2(0, 0));
	pT->setType(ProgressTimerType::BAR);
	pT->setBarChangeRate(Point(1, 0));
	pT->setMidpoint(Point(0, 1));
	pT->setPercentage(100);
	pT->setName("pT");
	pT->setPosition(Vec2(origin.x + 14 * pT->getContentSize().width, origin.y + visibleSize.height - 2 * pT->getContentSize().height));
	addChild(pT, 1);
	sp0->setAnchorPoint(Vec2(0, 0));
	sp0->setPosition(Vec2(origin.x + pT->getContentSize().width, origin.y + visibleSize.height - sp0->getContentSize().height));
	sp0->setName("sp0");
	addChild(sp0, 0);

    return true;
}


void HelloWorld::move(int direction) {
	//1 for w, 2 for a, 3 for s, 4 for d
	Vector<SpriteFrame*> Move;
	auto texture3 = Director::getInstance()->getTextureCache()->addImage("$lucia_forward.png");
	auto root = CCDirector::sharedDirector()->getRunningScene();
	auto location = (root->getChildByName("player"))->getPosition();
	auto player = (Sprite*)root->getChildByName("player");
	// 运动动画(帧数：8帧，高：101，宽：68）
	Move.reserve(8);
	for (int i = 0; i < 8; i++) {
		auto frame = SpriteFrame::createWithTexture(texture3, CC_RECT_PIXELS_TO_POINTS(Rect(68 * i, 0, 68, 101)));
		Move.pushBack(frame);
	}
	
	auto moveVector = Vec2(0, 0);
	switch (direction) {
		case 1:
			if(location.y + 30 < visibleSize.height)
				moveVector = Vec2(0, 30);
			break;
		case 2:
			if (location.x - 30 > 0)
				moveVector = Vec2(-30, 0);
			break;
		case 3:
			if (location.y - 30 > 0)
				moveVector = Vec2(0, -30);
			break;
		case 4:
			if (location.x + 30 < visibleSize.width)
				moveVector = Vec2(30, 0);
	}
	auto movement = MoveTo::create(0.4f, location + moveVector);
	auto animation = Animation::createWithSpriteFrames(Move, 0.05f);
	auto animate = Animate::create(animation);
	auto all = Spawn::create(animate, movement, NULL, NULL);
	player->stopAllActions();
	player->runAction(Repeat::create(all, 1));
	
}

void HelloWorld::playAttack() {
	Vector<SpriteFrame*> attack;
	auto texture = Director::getInstance()->getTextureCache()->addImage("$lucia_2.png");
	auto frame0 = SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(0, 0, 113, 113)));
	// 攻击动画
	attack.reserve(17);
	for (int i = 0; i < 17; i++) {
		auto frame = SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(113 * i, 0, 113, 113)));
		attack.pushBack(frame);
	}
	attack.pushBack(frame0);

	auto animation = Animation::createWithSpriteFrames(attack, 0.1f);
	auto animate = Animate::create(animation);

	auto root = CCDirector::sharedDirector()->getRunningScene();
	Sprite* player = (Sprite*)(root->getChildByName("player"));
	auto pT = (ProgressTimer*)root->getChildByName("pT");
	if (pT->getPercentage() <= 90)
		bloodChange(10);
	player->stopAllActions();
	player->runAction(Repeat::create(animate, 1));
}

void HelloWorld::playDead() {
	Vector<SpriteFrame*> dead;
	auto texture2 = Director::getInstance()->getTextureCache()->addImage("$lucia_dead.png");
	auto frame0 = SpriteFrame::createWithTexture(texture2, CC_RECT_PIXELS_TO_POINTS(Rect(0, 0, 79, 90)));
	// 死亡动画(帧数：22帧，高：90，宽：79）
	dead.reserve(22);
	for (int i = 0; i < 22; i++) {
		auto frame = SpriteFrame::createWithTexture(texture2, CC_RECT_PIXELS_TO_POINTS(Rect(79 * i, 0, 79, 90)));
		dead.pushBack(frame);
	}
	dead.pushBack(frame0);

	auto animation = Animation::createWithSpriteFrames(dead, 0.1f);
	auto animate = Animate::create(animation);

	auto root = CCDirector::sharedDirector()->getRunningScene();
	Sprite* player = (Sprite*)(root->getChildByName("player"));
	auto pT = (ProgressTimer*)root->getChildByName("pT");
	if (pT->getPercentage() >= 0) {
		bloodChange(-10);
	}
	player->stopAllActions();
	player->runAction(Repeat::create(animate, 1));
}

void HelloWorld::update(float tf) {
	auto root = CCDirector::sharedDirector()->getRunningScene();
	auto time = (Label*)root->getChildByName("time");
	auto now = atoi(time->getString().c_str());
	time->setString(Value(now - 1).asString());
}

void HelloWorld::bloodChange(int num) {
	auto root = CCDirector::sharedDirector()->getRunningScene();
	auto pT = (ProgressTimer*)root->getChildByName("pT");
	Sprite* sp0 = (Sprite*)(root->getChildByName("sp0"));
	
	auto action = ProgressTo::create(0.5f, pT->getPercentage() + num);
	pT->runAction(Repeat::create(action, 1));
}

