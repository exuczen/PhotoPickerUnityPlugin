//
//  iOSPhotoAndCamera.mm
//  UnityCameraPlugin
//
//  Created by Andre Schouten on 20.04.17.
//  Copyright © 2017 Andre Schouten. All rights reserved.
//

#import "iOSPhotoAndCamera.h"

@interface iOSPhotoAndCamera ()

@property(nonatomic) bool allowedEditing;

@end

@implementation iOSPhotoAndCamera

extern void UnitySendMessage(const char *, const char *, const char *);

-(id) init {
    self = [super init];
    return self;
}

- (void) ChoosePhoto:(bool)fromLibrary andAllowEditing:(bool)allowEditing {
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    picker.delegate = self;
    picker.allowsEditing = NO;
    _allowedEditing = allowEditing;

    if (fromLibrary) {
        picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    } else {
        picker.sourceType = UIImagePickerControllerSourceTypeCamera;
    }

    [[[[UIApplication sharedApplication] keyWindow] rootViewController] presentViewController:picker animated:YES completion:NULL];
}

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *)info {

    [picker dismissViewControllerAnimated:YES completion:NULL];

    UIImage *image = info[UIImagePickerControllerOriginalImage];
    if (_allowedEditing) {
        [self cropImage:image];
    } else {
        [self sendImageBackToUnity:image];
    }
}

- (void)cropImage:(UIImage*)image {
    TOCropViewController *cropViewController = [[TOCropViewController alloc] initWithImage:image];
    cropViewController.delegate = self;

    [cropViewController setAspectRatioLockEnabled:YES];
    cropViewController.aspectRatioPreset = TOCropViewControllerAspectRatioPresetSquare;

    [[[[UIApplication sharedApplication] keyWindow] rootViewController] presentViewController:cropViewController animated:YES completion:nil];
}

- (void)cropViewController:(TOCropViewController *)cropViewController didCropToImage:(UIImage *)image withRect:(CGRect)cropRect angle:(NSInteger)angle {
    [cropViewController dismissViewControllerAnimated:YES completion:NULL];
    [self sendImageBackToUnity:image];
}

- (void)cropViewController:(TOCropViewController *)cropViewController didFinishCancelled:(BOOL)cancelled {
    if (cancelled)
        UnitySendMessage("[iOSPhotoAndCamera]", "_DidCancel", "USER_CANCELLED");
}

- (void)sendImageBackToUnity:(UIImage *)image {
    NSData *imageData = UIImagePNGRepresentation(image);
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    NSString *filePath = [documentsDirectory stringByAppendingPathComponent:@"shared_iosphotoandcamera.png"];
    [imageData writeToFile:filePath atomically:YES];

    UnitySendMessage("[iOSPhotoAndCamera]", "_DidFinishPickingMedia", [filePath UTF8String]);
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    UnitySendMessage("[iOSPhotoAndCamera]", "_DidCancel", "USER_CANCELLED");
    [picker dismissViewControllerAnimated:YES completion:NULL];
}

@end

static iOSPhotoAndCamera* instance = nil;

extern "C" {
    void _ChoosePhoto (bool fromLibrary, bool allowEditing) {
        if(instance == nil)
            instance = [[iOSPhotoAndCamera alloc] init];

        [instance ChoosePhoto:fromLibrary andAllowEditing:allowEditing];
    }
}
