
typedef void (*CallbackFunc)(void);

CallbackFunc saved_callback = 0;

void saveCallback(CallbackFunc func)
{
	saved_callback = func;
}

void testSavedCallback()
{
	saved_callback();
}

void testCallback(CallbackFunc func)
{
	func();
}