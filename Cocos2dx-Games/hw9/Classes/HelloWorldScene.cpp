#include "HelloWorldScene.h"
#include "SimpleAudioEngine.h"

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

void specialLabel3(Node* temp) {
	auto label = (Label*)temp;
	Sprite* sp1 = label->getLetter(0);
	sp1->setScale(2.0f);
	sp1->setColor(Color3B::GREEN);

	Sprite* sp2 = label->getLetter(11);
	sp2->setColor(Color3B::RED);
	sp2->runAction(RepeatForever::create(RotateBy::create(1.0f, 90)));
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

	isSYSU = true;
    auto visibleSize = Director::getInstance()->getVisibleSize();
    Vec2 origin = Director::getInstance()->getVisibleOrigin();

    /////////////////////////////
    // 2. add a menu item with "X" image, which is clicked to quit the program
    //    you may modify it.

    // add a "close" icon to exit the progress. it's an autorelease object
    auto closeItem = MenuItemImage::create(
                                           "CloseNormal.png",
                                           "CloseSelected.png",
                                           CC_CALLBACK_1(HelloWorld::menuCloseCallback, this));
	auto clickLabel = Label::createWithTTF("Click Me", "fonts/Marker Felt.ttf", 25);
	clickLabel->enableOutline(Color4B::ORANGE, 3);
	auto clickItem = MenuItemLabel::create(clickLabel, CC_CALLBACK_1(HelloWorld::menuClickCallback, this));
	

    if (closeItem == nullptr ||
        closeItem->getContentSize().width <= 0 ||
        closeItem->getContentSize().height <= 0)
    {
        problemLoading("'CloseNormal.png' and 'CloseSelected.png'");
    }
    else
    {
        float x = origin.x + visibleSize.width - closeItem->getContentSize().width/2;
        float y = origin.y + closeItem->getContentSize().height/2;
        closeItem->setPosition(Vec2(x,y));
		clickItem->setPosition(Vec2(x - 30, y + 50));
    }

    // create menu, it's an autorelease object
    auto menu1 = Menu::create(closeItem, NULL);
	auto menu2 = Menu::create(clickItem, NULL);
    menu1->setPosition(Vec2::ZERO);
	menu2->setPosition(Vec2::ZERO);
    this->addChild(menu1, 1);
	this->addChild(menu2, 1);

    /////////////////////////////
    // 3. add your codes below...

    // add a label shows "Hello World"
    // create and initialize a label

	Dictionary* dic = Dictionary::createWithContentsOfFile("xml/strings.xml");
	const char* name = ((String*)dic->objectForKey("name"))->getCString();
	const char* id = ((String*)dic->objectForKey("StudentID"))->getCString();
	
	
    auto labelOne = CCLabelTTF::create(name, "msyh", 25);
	auto labelTwo = Label::create(id, "fonts/Marker Felt.ttf", 25);
	auto labelThree = Label::create("Hello, Cocos", "fonts/Marker Felt.ttf", 32);
	specialLabel3(labelThree);
	labelOne->setTag(1);
	labelTwo->setTag(2);
	labelThree->setTag(3);
	labelThree->setVisible(false);

	labelOne->setColor(Color3B::RED);
	labelTwo->setColor(Color3B::GREEN);

    if (labelOne == nullptr || labelTwo == nullptr)
    {
        problemLoading("'fonts/Marker Felt.ttf'");
    }
    else
    {
        // position the label on the center of the screen
        labelOne->setPosition(Vec2(origin.x + visibleSize.width/2, 
									origin.y + visibleSize.height - labelOne->getContentSize().height - 50));
		labelTwo->setPosition(Vec2(origin.x + visibleSize.width / 2,
									origin.y + visibleSize.height - labelTwo->getContentSize().height - 100));
		labelThree->setPosition(Vec2(origin.x + visibleSize.width / 2 + 300,
			origin.y + visibleSize.height - labelTwo->getContentSize().height - 120));

        // add the label as a child to this layer
        this->addChild(labelOne, 1);
		this->addChild(labelTwo, 1);
		this->addChild(labelThree, 1);
    }

    // add "HelloWorld" splash screen"
    auto sprite = Sprite::create("img/zsdx.jpg");
	sprite->setTag(0);
	
    if (sprite == nullptr)
    {
        problemLoading("'img/sky.jpg'");
    }
    else
    {
        // position the sprite on the center of the screen
        sprite->setPosition(Vec2(visibleSize.width/2 + origin.x, visibleSize.height/2 + origin.y));

        // add the sprite as a child to this layer
        this->addChild(sprite, 0);
    }
    return true;
}


void HelloWorld::menuCloseCallback(Ref* pSender)
{
    //Close the cocos2d-x game scene and quit the application
    Director::getInstance()->end();

    #if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
    exit(0);
#endif

    /*To navigate back to native iOS screen(if present) without quitting the application  ,do not use Director::getInstance()->end() and exit(0) as given above,instead trigger a custom event created in RootViewController.mm as below*/

    //EventCustom customEndEvent("game_scene_close_event");
    //_eventDispatcher->dispatchEvent(&customEndEvent);


}

void exchangeColors(Node* node1, Node* node2) {
	LabelTTF* nodeOne = (LabelTTF*)node1;
	Label* nodeTwo = (Label*)node2;
	auto str1 = nodeOne->getColor();
	auto str2 = nodeTwo->getColor();
	nodeOne->setColor(str2);
	nodeTwo->setColor(str1);
}

void findImageSprite(Scene* root, bool& isImg) {
	Sprite* node = (Sprite*)root->getChildByTag(0);
	if (isImg) {
		Texture2D* newImg = TextureCache::sharedTextureCache()->addImage("img/HelloWorld.png");
		node->setTexture(newImg);
		isImg = false;
		((Label*)root->getChildByTag(3))->setVisible(true);
	}
	else {
		Texture2D* newImg = TextureCache::sharedTextureCache()->addImage("img/zsdx.jpg");
		node->setTexture(newImg);
		isImg = true;
		((Label*)root->getChildByTag(3))->setVisible(false);
	}
	exchangeColors(root->getChildByTag(1), root->getChildByTag(2));
}

void HelloWorld::menuClickCallback(Ref* pSender) {
	auto allNodes = CCDirector::sharedDirector()->getRunningScene();
	findImageSprite(allNodes, isSYSU);
}
