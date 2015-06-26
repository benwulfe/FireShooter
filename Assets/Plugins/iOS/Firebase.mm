
#import "Firebase/Firebase.h"

// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

// Helper method to create C string copy
char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

extern "C" {
    typedef void (*OnValueChanged)(void*, void* );
    
    void* _FirebaseNew (const char *path)
    {
        // Create a reference to a Firebase location
        Firebase *myFirebaseRef = [[Firebase alloc] initWithUrl:CreateNSString(path)];
        return (void*)CFBridgingRetain(myFirebaseRef);
    }
    
    void _FirebaseRelease(void* firebase) {
        Firebase *myFirebaseRef = (__bridge_transfer Firebase*) (firebase);
    }
    
    void* _FirebaseChild(void* firebase, const char *path)
    {
        Firebase *myFirebaseRef = (__bridge Firebase*) (firebase);
        Firebase *child = [myFirebaseRef childByAppendingPath:CreateNSString(path)];
        if (child == nil) {
            return nil;
        }
        return (void*)CFBridgingRetain(child);
    }
    
    void* _FirebaseParent(void* firebase)
    {
        Firebase *myFirebaseRef = (__bridge Firebase*) (firebase);
        Firebase *parent = [myFirebaseRef parent];
        if (parent == nil) {
            return nil;
        }
        return (void*)CFBridgingRetain(parent);
    }
    
    void* _FirebaseRoot(void* firebase)
    {
        Firebase *myFirebaseRef = (__bridge Firebase*) (firebase);
        Firebase *root = [myFirebaseRef root];
        if (root == nil) {
            return nil;
        }
        return (void*)CFBridgingRetain(root);
    }
    
    const char* _FirebaseGetKey(void* firebase)
    {
        Firebase *myFirebaseRef = (__bridge Firebase*) (firebase);
        return MakeStringCopy([[myFirebaseRef key] UTF8String]);
    }
    
    void* _FirebasePush(void* firebase)
    {
        Firebase *myFirebaseRef = (__bridge Firebase*) (firebase);
        Firebase *push = [myFirebaseRef childByAutoId];
        if (push == nil) {
            return nil;
        }
        return (void*)CFBridgingRetain(push);
    }
    
    void _FirebaseSetString(void* firebase, const char *value) {
        Firebase *myFirebaseRef = (__bridge Firebase*) (firebase);
        [myFirebaseRef setValue:CreateNSString(value)];
    }
    
    void _FirebaseSetJson(void* firebase, const char *value) {
        Firebase *myFirebaseRef = (__bridge Firebase*) (firebase);
        NSData *data = [CreateNSString(value) dataUsingEncoding:NSUTF8StringEncoding];
        id json = [NSJSONSerialization JSONObjectWithData:data options:0 error:nil];
        
        [myFirebaseRef setValue:json];
    }
    
    void _FirebaseSetFloat(void* firebase, float value) {
        Firebase *myFirebaseRef = (__bridge Firebase*) (firebase);
        [myFirebaseRef setValue:[NSNumber numberWithFloat: value]];
    }
    
    void _FirebaseSetPriority(void* firebase, const char *value) {
        Firebase *myFirebaseRef = (__bridge Firebase*) (firebase);
        [myFirebaseRef setPriority:CreateNSString(value)];
    }
    
    void _FirebaseObserveValueChange( void* firebase, OnValueChanged onChanged, void* refId) {
        Firebase *myFirebaseRef = (__bridge Firebase*) (firebase);
        [myFirebaseRef observeEventType:FEventTypeValue withBlock:^(FDataSnapshot *snapshot) {
            onChanged((void*)CFBridgingRetain(snapshot), refId);
        }];
    }
    
    void _FirebaseObserveChildAdded( void* firebase, OnValueChanged onChanged, void* refId) {
        Firebase *myFirebaseRef = (__bridge Firebase*) (firebase);
        [myFirebaseRef observeEventType:FEventTypeChildAdded withBlock:^(FDataSnapshot *snapshot) {
            onChanged((void*)CFBridgingRetain(snapshot), refId);
        }];
    }
    
    float _DataSnapshotGetFloatValue (void* datasnapshot) {
        FDataSnapshot *snapshotRef = (__bridge FDataSnapshot*) (datasnapshot);
        return [[snapshotRef value] floatValue];
    }
    
    const char* _DataSnapshotGetStringValue (void* datasnapshot) {
        FDataSnapshot *snapshotRef = (__bridge FDataSnapshot*) (datasnapshot);
        return MakeStringCopy([[snapshotRef value] UTF8String]);
    }
    
    const char* _DataSnapshotGetDictionary(void* datasnapshot) {
        FDataSnapshot *snapshotRef = (__bridge FDataSnapshot*) (datasnapshot);
        id value = [snapshotRef value];
        if([value isKindOfClass:[NSDictionary class]]
           && [NSJSONSerialization isValidJSONObject:value]) {
            NSError *error;
            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:value
                                                               options:0
                                                                 error:&error];
            if (! jsonData) {
                return nil;
            } else {
                NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                return MakeStringCopy([jsonString UTF8String]);
            }
        }
        return nil;
    }
    
    void* _DataSnapshotGetChild (void* datasnapshot, const char* path) {
        FDataSnapshot *snapshotRef = (__bridge FDataSnapshot*) (datasnapshot);
        return (void*)CFBridgingRetain([snapshotRef childSnapshotForPath:CreateNSString(path)]);
    }
    
    void* _DataSnapshotHasChild (void* datasnapshot, const char* path) {
        FDataSnapshot *snapshotRef = (__bridge FDataSnapshot*) (datasnapshot);
        if ([snapshotRef hasChild:CreateNSString(path)]) {
            return (void*)(1);
        }
        return nil;
    }
    
    void* _DataSnapshotExists (void* datasnapshot) {
        FDataSnapshot *snapshotRef = (__bridge FDataSnapshot*) (datasnapshot);
        if (snapshotRef.value != [NSNull null]) {
            return (void*)1;
        }
        return nil;
    }
    
    const char* _DataSnapshotGetKey (void* datasnapshot) {
        FDataSnapshot *snapshotRef = (__bridge FDataSnapshot*) (datasnapshot);
        return MakeStringCopy([[snapshotRef key] UTF8String]);
    }
    
    const char* _DataSnapshotGetPriority (void* datasnapshot) {
        FDataSnapshot *snapshotRef = (__bridge FDataSnapshot*) (datasnapshot);
        return MakeStringCopy([[snapshotRef priority] UTF8String]);
    }
    
    void* _DataSnapshotGetRef (void* datasnapshot) {
        FDataSnapshot *snapshotRef = (__bridge FDataSnapshot*) (datasnapshot);
        return (void*)CFBridgingRetain([snapshotRef ref]);
    }
    
    void _DataSnapshotRelease(void* datasnapshot) {
        FDataSnapshot *snapshotRef = (__bridge_transfer FDataSnapshot*) (datasnapshot);
    }
}

