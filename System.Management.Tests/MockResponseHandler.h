
#include <UNIX_Common.h>

#ifndef __MOCK_RESPONSEHANDLER_H
#define __MOCK_RESPONSEHANDLER_H

class MockResponseHandler : virtual public ResponseHandler
{
public:
	MockResponseHandler(String testName);
	~MockResponseHandler();
	virtual void processing(); 
	virtual void complete();
	virtual bool wasHandled();

private:
	bool _processed;
	bool _completed;
	String _testName;
};

class MockObjectPathResponseHandler : virtual public ObjectPathResponseHandler
{
public:
	MockObjectPathResponseHandler(String testName);
	~MockObjectPathResponseHandler();
	virtual void processing(); 
	virtual void complete();
	virtual void deliver(const CIMObjectPath&);
	virtual void deliver(const Array<CIMObjectPath>&);
	virtual bool wasHandled();

private:
	bool _processed;
	bool _completed;
	bool _delivered;
	String _testName;
};


class MockMethodResultResponseHandler : virtual public MethodResultResponseHandler
{
public:
	MockMethodResultResponseHandler();
	~MockMethodResultResponseHandler();
	virtual void processing(); 
	virtual void complete();
	virtual void deliverParamValue(const CIMParamValue&);
	virtual void deliverParamValue(const Array<CIMParamValue>&);
	virtual void deliver(const CIMValue&);
};

#endif