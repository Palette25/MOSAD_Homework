#include "MenuScene.h"
#include "SimpleAudioEngine.h"
#include "GameScene.h"

USING_NS_CC;

Scene* MenuScene::createScene()
{
    return MenuScene::create();
}

// Print useful error message instead of segfaulting when files are not there.
static void problemLoading(const char* filename)
{
    printf("Error while loading: %s\n", filename);
    printf("Depending on how you compiled you might have to add 'Resources/' in front of filenames in HelloWorldScene.cpp\n");
}

// on "init" you need to initialize your instance
bool MenuScene::init()
{
    //////////////////////////////
    // 1. super init first
    if ( !Scene::init() )
    {
        return false;
    }

    auto visibleSize = Director::getInstance()->getVisibleSize();
    Vec2 origin = Director::getInstance()->getVisibleOrigin();

	auto bg_sky = Sprite::create("menu-background-sky.jpg");
	bg_sky->setPosition(Vec2(visibleSize.width / 2 + origin.x, visibleSize.height / 2 + origin.y + 150));
	this->addChild(bg_sky, 0);

	auto bg = Sprite::create("menu-background.png");
	bg->setPosition(Vec2(visibleSize.width / 2 + origin.x, visibleSize.height / 2 + origin.y - 60));
	this->addChild(bg, 0);

	auto miner = Sprite::create("menu-miner.png");
	miner->setPosition(Vec2(150 + origin.x, visibleSize.height / 2 + origin.y - 60));
	this->addChild(miner, 1);

	//Start game click button
	auto clickItem = MenuItemImage::create(
										"start-0.png",
										"start-1.png",
										CC_CALLBACK_1(MenuScene::startGame, this));
	float x = origin.x + visibleSize.width - clickItem->getContentSize().width / 2;
	float y = origin.y + clickItem->getContentSize().height / 2;
	clickItem->setPosition(Vec2(x - 80, y + 180));
	//Start game button background stone image
	auto stone = Sprite::create("menu-start-gold.png");
	auto title = Sprite::create("gold-miner-text.png");
	stone->setPosition(Vec2(x - 90, y + 130));
	title->setPosition(Vec2(x - 400, y + 450));
	this->addChild(stone, 0);
	this->addChild(title, 0);

	auto menu1 = Menu::create(clickItem, NULL);
	menu1->setPosition(Vec2::ZERO);
	this->addChild(menu1);
	
	auto leg = Sprite::createWithSpriteFrameName("miner-leg-0.png");
	Animate* legAnimate = Animate::create(AnimationCache::getInstance()->getAnimation("legAnimation"));
	leg->runAction(RepeatForever::create(legAnimate));
	leg->setPosition(110 + origin.x, origin.y + 102);
	this->addChild(leg, 1);

    return true;
}

void MenuScene::startGame(Ref* pSender){
	Scene* s = GameSence::createScene();
	auto animateScene = TransitionFade::create(1.5f, s);
	CCDirector::sharedDirector()->replaceScene(animateScene);
}

