#include "GameScene.h"

USING_NS_CC;

Scene* GameSence::createScene()
{
	return GameSence::create();
}

// on "init" you need to initialize your instance
bool GameSence::init()
{
	//////////////////////////////
	// 1. super init first
	if (!Scene::init())
	{
		return false;
	}

	auto shootLabel = Label::createWithTTF("Shoot", "fonts/Marker Felt.ttf", 50);
	auto shootItem = MenuItemLabel::create(shootLabel, CC_CALLBACK_1(GameSence::shootMouse, this));

	//add touch listener
	EventListenerTouchOneByOne* listener = EventListenerTouchOneByOne::create();
	listener->setSwallowTouches(true);
	listener->onTouchBegan = CC_CALLBACK_2(GameSence::onTouchBegan, this);
	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(listener, this);

	auto visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();
	auto stoneLayer = Vec2(0, 0);
	auto mouseLayer = Vec2(0, visibleSize.height / 2);

	//Background Image
	auto bg = Sprite::create("level-background-0.jpg");
	bg->setPosition(Vec2(visibleSize.width / 2 + origin.x, visibleSize.height / 2 + origin.y));
	this->addChild(bg, 0);

	//Stone and Mouse, as well as tnt
	auto stone = Sprite::createWithSpriteFrameName("stone-0.png");
	auto mouse = Sprite::createWithSpriteFrameName("gem-mouse-0.png");
	auto TNT = Sprite::createWithSpriteFrameName("TNT.png");
	auto soil = Sprite::createWithSpriteFrameName("soil-0.png");
	auto shootMenu = Menu::create(shootItem, NULL);

	//Animation definitions
	Animate* mouseAnimate = Animate::create(AnimationCache::getInstance()->getAnimation("mouseAnimation"));
	Animate* stoneAnimate = Animate::create(AnimationCache::getInstance()->getAnimation("stoneAnimation"));
	Animate* tntAnimation = Animate::create(AnimationCache::getInstance()->getAnimation("tntAnimation"));
	Animate* soilAnimation = Animate::create(AnimationCache::getInstance()->getAnimation("soilAnimation"));

	mouse->runAction(RepeatForever::create(mouseAnimate));
	stone->runAction(RepeatForever::create(stoneAnimate));
	TNT->runAction(RepeatForever::create(tntAnimation));
	soil->runAction(RepeatForever::create(soilAnimation));

	//Position definition
	stone->setPosition(Vec2(stoneLayer.x + 560, stoneLayer.y + 480));
	mouse->setPosition(Vec2(mouseLayer.x + visibleSize.width / 2, mouseLayer.y));
	shootItem->setPosition(Vec2(stoneLayer.x + 750, stoneLayer.y + 480));
	TNT->setPosition(Vec2(stoneLayer.x + 100, stoneLayer.y + 480));
	soil->setPosition(Vec2(stoneLayer.x + 200, stoneLayer.y + 400));
	shootMenu->setPosition(Vec2::ZERO);

	//Names definition
	stone->setName("stone");
	mouse->setName("mouse");

	//Add to scene
	this->addChild(stone, 0);
	this->addChild(mouse, 0);
	this->addChild(TNT, 0);
	this->addChild(soil, 0);
	this->addChild(shootMenu, 1);

	return true;
}

bool GameSence::onTouchBegan(Touch *touch, Event *unused_event) {
	auto location = touch->getLocation();
	//Judge whether is in soil area
	if (location.y > 450) {
		return false;
	}

	//Produce Cheese
	auto cheese = Sprite::create("cheese.png");
	cheese->setPosition(Vec2(location.x, location.y));
	this->addChild(cheese, 1);

	//Mouse movement
	makeLineMove("mouse", location, cheese);

	return true;
}

void GameSence::shootMouse(Ref* pSender) {
	//Set stone layer
	auto stoneLayer = Vec2(0, 0);
	
	//Get mouse location
	auto root = (Sprite*)CCDirector::getInstance()->getRunningScene();
	auto mouse = (Sprite*)root->getChildByName("mouse");
	auto mLocation = mouse->getPosition();

	//Create new stone bullet
	auto newStone = Sprite::create("stone.png");
	newStone->setPosition(Vec2(stoneLayer.x + 560, stoneLayer.y + 480));
	this->addChild(newStone, 1);

	//Stone movement
	Point moveVector = mLocation - newStone->getPosition();
	Point normVectoor = ccpNormalize(moveVector);
	Point overVector = normVectoor * 900;
	float moveSpeed = 500; //Move Speed of mouse
	float moveTime = overVector.length() / moveSpeed; //Total time of movement

	auto movement = MoveTo::create(moveTime, newStone->getPosition() + moveVector);
	auto moveDone = CallFunc::create(CC_CALLBACK_0(GameSence::moveFinish, this, newStone));
	newStone->runAction(Sequence::create(movement, moveDone, NULL));

	//Mouse movement
	auto ranX = CCRANDOM_0_1()*400 + 200;
	auto ranY = CCRANDOM_0_1()*400 + 200;
	Point newLocation = Vec2(ranX, ranY);
	makeLineMove("mouse", newLocation, NULL);

	//Diamond leavement
	auto diamond = Sprite::create("diamond.png");
	diamond->setPosition(mLocation);
	this->addChild(diamond, 0);
}

void GameSence::makeLineMove(std::string name, Point target, Node* cheese) {
	auto root = (Sprite*)CCDirector::getInstance()->getRunningScene();
	auto ele = (Sprite*)root->getChildByName(name);
	Point moveVector = target - ele->getPosition();
	Point normVectoor = ccpNormalize(moveVector);
	Point overVector = normVectoor * 900;
	float moveSpeed = 500; //Move Speed of mouse
	float moveTime = overVector.length() / moveSpeed; //Total time of movement
	auto dele = cheese;
	//Movement
	auto movement = MoveTo::create(moveTime, ele->getPosition() + moveVector);
	auto moveDone = CallFunc::create(std::bind(&GameSence::moveFinish, this, (Node*)dele));
	ele->runAction(Sequence::create(movement, moveDone, NULL));
}

void GameSence::moveFinish(Node* ele) {
	auto sele = (Sprite*)ele;
	if (sele == NULL)
		return;
	//Wait until done
	auto fadeout = CCFadeOut::create(1);
	sele->runAction(Repeat::create(fadeout, 1));
}
