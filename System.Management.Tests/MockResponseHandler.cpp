
#include "MockResponseHandler.h"
#include <iostream>

MockResponseHandler::MockResponseHandler(String testName)
{
	_processed = false;
	_completed = false;
	_testName = testName;
}

MockResponseHandler::~MockResponseHandler()
{

}

void MockResponseHandler::processing()
{
	_processed = true;
}

void MockResponseHandler::complete()
{
	_completed = true;
}

bool MockResponseHandler::wasHandled()
{
	if (_processed && _completed)
	{
		cout << _testName << " was handled correctly." << endl;
	}
	return _processed && _completed;
}

MockObjectPathResponseHandler::MockObjectPathResponseHandler(String testName)
{
	_processed = false;
	_completed = false;
	_delivered = true;
	_testName = testName;
}

MockObjectPathResponseHandler::~MockObjectPathResponseHandler()
{

}

void MockObjectPathResponseHandler::processing()
{
	_processed = true;	
}

void MockObjectPathResponseHandler::complete()
{
	_completed = true;
}

void MockObjectPathResponseHandler::deliver(const CIMObjectPath &value)
{
	_delivered = true;
}

void MockObjectPathResponseHandler::deliver(const Array<CIMObjectPath> &values)
{

}

bool MockObjectPathResponseHandler::wasHandled()
{
	if (_processed && _completed && _delivered)
	{
		cout << _testName << " was handled correctly." << endl;
	}
	return _processed && _completed && _delivered;
}

MockMethodResultResponseHandler::MockMethodResultResponseHandler()
{

}

MockMethodResultResponseHandler::~MockMethodResultResponseHandler()
{

}

void MockMethodResultResponseHandler::processing()
{
	
}

void MockMethodResultResponseHandler::complete()
{
	
}

void MockMethodResultResponseHandler::deliverParamValue(const CIMParamValue &value)
{

}

void MockMethodResultResponseHandler::deliverParamValue(const Array<CIMParamValue> &values)
{

}

void MockMethodResultResponseHandler::deliver(const CIMValue &value)
{

}