//
//  iOSPhotoAndCamera.h
//  UnityCameraPlugin
//
//  Created by Andre Schouten on 20.04.17.
//  Copyright © 2017 Andre Schouten. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "TOCropViewController.h"

@interface iOSPhotoAndCamera : NSObject <TOCropViewControllerDelegate, UINavigationControllerDelegate, UIImagePickerControllerDelegate>

@end
